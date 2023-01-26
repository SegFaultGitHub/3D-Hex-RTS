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
        private MouseController.MouseController MouseController;
        private ResourcesManager.ResourcesManager ResourcesManager;
        public const int GOLD_COST = 50;
        public const int WOOD_COST = 35;

        protected override void Awake() {
            base.Awake();
            this.Behaviour = _Behaviour.Idle;
            this.LastChop = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            this.MouseController = GameObject.FindGameObjectWithTag("MouseController").GetComponent<MouseController.MouseController>();
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

            this.RotateTowardsTile(this.TreeTile);

            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (this.LastChop + this.ChopCooldown > now) return;
            this.LastChop = now;

            Trees trees = this.TreeTile.GetComponentInChildren<Trees>();
            if (trees is null) {
                this.FindNearestTrees();
                return;
            }
            this.Animator.SetTrigger(ATTACK);
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
            this.ReturnToCastle(this.FindNearestCastle());
        }

        private void ReturnToCastle(Tile castle) {
            this.Behaviour = _Behaviour.Storing;
            this.Castle = castle;
            this.SetPath(castle);
        }

        public override void InteractWith(IInteractable interactable) {
            if (interactable == null) return;
            switch (interactable) {
                case Trees trees:
                    this.Behaviour = _Behaviour.Chopping;
                    this.TreeTile = trees.Tile;
                    this.TreeTile.Feedback();
                    this.SetPath(this.TreeTile);
                    this.MouseController.Deselect(this);
                    break;
                case Castle castle:
                    this.TreeTile = null;
                    this.ReturnToCastle(castle.Tile);
                    break;
            }
        }

        public override void GoToTile(Tile destination) {
            base.GoToTile(destination);
            this.Behaviour = _Behaviour.Idle;
            this.TreeTile = null;
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
