using System;
using System.Collections.Generic;
using Code.Interactable;
using Code.Tiles;
using Code.UI;
using UnityEngine;
using Building = Code.Enums.Building;

namespace Code.Characters {
    public class Builder : Player {

        private static readonly int ATTACK = Animator.StringToHash("Attack");
        [field: SerializeField] private Tile BuildingTile;
        [field: SerializeField] private long BuildCooldown;
        [field: SerializeField] private int BuildPower = 10;

        private _Behaviour Behaviour;

        private BuilderUI BuilderUI;
        private long LastBuild;
        private MouseController.MouseController MouseController;
        private ResourcesManager.ResourcesManager ResourcesManager;

        private LTDescr Tween;
        public override string Name => "Builder";
        public override int GoldCost => 55;
        public override int WoodCost => 55;

        [field: SerializeField] public List<Building> Buildings { get; private set; }

        protected override void Awake() {
            base.Awake();
            this.Behaviour = _Behaviour.Idle;
            this.LastBuild = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            this.BuilderUI = this.GetComponentInChildren<BuilderUI>();

            this.ResourcesManager = GameObject.FindGameObjectWithTag("ResourcesManager").GetComponent<ResourcesManager.ResourcesManager>();
            this.MouseController = GameObject.FindGameObjectWithTag("MouseController").GetComponent<MouseController.MouseController>();
        }

        protected override void Update() {
            base.Update();
            if (this.BuildingTile is not null
                && this.Behaviour == _Behaviour.Building
                && this.Path.Tiles.Count == 0
                && this.GroundTile.DistanceFrom(this.BuildingTile) == 1)
                this.Build();
        }

        private void Build() {
            this.RotateTowardsTile(this.BuildingTile);

            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (this.LastBuild + this.BuildCooldown > now) return;
            this.LastBuild = now;

            Interactable.Building building = this.BuildingTile.GetComponentInChildren<Interactable.Building>();
            if (building.Completed) {
                this.BuildingTile = null;
                this.Behaviour = _Behaviour.Idle;
                return;
            }
            this.Animator.SetTrigger(ATTACK);
            building.AddDurability(this.BuildPower);
            // ReSharper disable once InvertIf
            if (building.Completed) {
                this.BuildingTile = null;
                this.Behaviour = _Behaviour.Idle;
            }
        }

        public override void InteractWith(IInteractable interactable) {
            if (interactable == null) return;
            switch (interactable) {
                case Interactable.Building { Completed: false } building:
                    this.Behaviour = _Behaviour.Building;
                    this.BuildingTile = building.Tile;
                    this.BuildingTile.Feedback();
                    this.SetPath(this.BuildingTile);
                    this.MouseController.Deselect(this);
                    break;
            }
        }

        public override void GoToTile(Tile destination) {
            base.GoToTile(destination);
            this.Behaviour = _Behaviour.Idle;
            this.BuildingTile = null;
        }

        public void CreatePhantomBuilding(PhantomBuilding phantom) {
            if (!phantom.OnCompletionBuilding.CanBuild(this.ResourcesManager)) return;

            this.MouseController.CreatePhantomBuilding(this, phantom);
            this.MouseController.Deselect(this);
        }

        public override void OnSelect() {
            this.MouseExit();

            this.BuilderUI.Open();
        }

        public override void OnDeselect() {
            base.MouseExit();

            this.BuilderUI.Close();
        }

        // ReSharper disable once InconsistentNaming
        private enum _Behaviour {
            Idle,
            Building
        }
    }
}
