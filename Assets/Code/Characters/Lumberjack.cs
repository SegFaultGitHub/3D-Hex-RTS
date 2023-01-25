using System;
using Code.Interactable;
using Code.Tiles;
using TMPro;
using UnityEngine;

namespace Code.Characters {
    public class Lumberjack : Player {


        private static readonly int ATTACK = Animator.StringToHash("Attack");
        [field: SerializeField] private int CarryingCapacity;
        [field: SerializeField] private int Carrying;
        [field: SerializeField] private Tile TreeTile;
        [field: SerializeField] private long ChopCooldown;
        [field: SerializeField] private TMP_Text CarryingText;
        private _Behaviour Behaviour;

        private Tile Castle;
        private long LastChop;
        private ResourcesManager.ResourcesManager ResourcesManager;
        public const int GOLD_COST = 50;
        public const int WOOD_COST = 35;

        protected override void Awake() {
            base.Awake();
            this.Behaviour = _Behaviour.Idle;
            this.LastChop = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            this.Castle = GameObject.FindGameObjectWithTag("Castle").GetComponent<Tile>();
            this.ResourcesManager = GameObject.FindGameObjectWithTag("ResourcesManager").GetComponent<ResourcesManager.ResourcesManager>();
            this.CarryingText.SetText(this.Carrying.ToString());
        }

        protected override void Update() {
            base.Update();
            if (this.TreeTile is not null && this.TreeTile.GetComponentInChildren<Trees>() is null)
                this.FindNearestTrees();
            if (this.TreeTile is not null
                && this.Behaviour == _Behaviour.Chopping
                && this.Path.Tiles.Count == 0
                && this.GroundTile.DistanceFrom(this.TreeTile) == 1)
                this.ChopDownTrees();
            if (this.Behaviour == _Behaviour.Storing && this.Path.Tiles.Count == 0 && this.GroundTile.DistanceFrom(this.Castle) == 1)
                this.StoreWood();
        }

        private void ChopDownTrees() {
            if (this.Carrying >= this.CarryingCapacity) {
                this.ReturnToCastle();
                return;
            }

            this.RotateTowardsTrees();

            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (this.LastChop + this.ChopCooldown > now) return;
            this.LastChop = now;

            this.Animator.SetTrigger(ATTACK);
            Trees trees = this.TreeTile.GetComponentInChildren<Trees>();
            TreeConsumption treeConsumption = trees.Consume(1);
            this.Carrying += treeConsumption.Quantity;
            this.CarryingText.SetText(this.Carrying.ToString());

            if (this.Carrying >= this.CarryingCapacity) this.ReturnToCastle();
            if (treeConsumption.Depleted) this.FindNearestTrees();
        }

        private void StoreWood() {
            this.ResourcesManager.AddWood(this.Carrying);
            this.Carrying = 0;
            this.CarryingText.SetText(this.Carrying.ToString());
            if (this.TreeTile is null) {
                this.Behaviour = _Behaviour.Idle;
            } else {
                this.Behaviour = _Behaviour.Chopping;
                this.SetPath(this.TreeTile);
            }
        }

        private void FindNearestTrees() {
            this.TreeTile = this.TreeTile.FindNearestTrees(this.Controller.stepOffset, 5);
            if (this.TreeTile is not null) this.SetPath(this.TreeTile);
        }

        private void ReturnToCastle() {
            this.Behaviour = _Behaviour.Storing;
            this.SetPath(this.Castle);
        }

        public override void InteractWith(IInteractable interactable) {
            if (interactable == null) return;
            switch (interactable) {
                case Trees trees:
                    this.Behaviour = _Behaviour.Chopping;
                    this.TreeTile = trees.GetComponentInParent<Tile>();
                    this.TreeTile.Feedback();
                    this.SetPath(this.TreeTile);
                    break;
                case Interactable.Castle:
                    this.TreeTile = null;
                    this.ReturnToCastle();
                    break;
            }
        }

        public override void GoToTile(Tile destination) {
            base.GoToTile(destination);
            this.Behaviour = _Behaviour.Idle;
            this.TreeTile = null;
        }

        private void RotateTowardsTrees() {
            Vector3 diff = this.TreeTile.GridPosition - this.GroundTile.GridPosition;
            float targetAngle;
            if (diff == new Vector3(1, -1, 0)) targetAngle = 30;
            else if (diff == new Vector3(1, 0, -1)) targetAngle = 90;
            else if (diff == new Vector3(0, 1, -1)) targetAngle = 150;
            else if (diff == new Vector3(-1, 1, 0)) targetAngle = 210;
            else if (diff == new Vector3(-1, 0, 1)) targetAngle = 270;
            else if (diff == new Vector3(0, -1, 1)) targetAngle = 330;
            else throw new Exception("[Lumberjack:RotateTowardsTrees] Invalid diff.");

            float angle = Mathf.SmoothDampAngle(this.transform.eulerAngles.y, targetAngle, ref this.TurnSmoothVelocity, TURN_SMOOTH_TIME);
            this.transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        public override bool CanSummon(ResourcesManager.ResourcesManager resourcesManager) {
            return resourcesManager.Gold >= GOLD_COST && resourcesManager.Wood >= WOOD_COST;
        }

        // ReSharper disable once InconsistentNaming
        private enum _Behaviour {
            Idle,
            Chopping,
            Storing
        }
    }
}
