using System;
using Code.Tiles;
using Code.UI;
using UnityEngine;

namespace Code.Interactable {
    public abstract class Building : Selectable, IInteractable {
        [field: SerializeField] private int Durability;
        [field: SerializeField] private int TotalDurability;
        protected BuildingNameUI BuildingNameUI;
        public abstract string Name { get; }
        public abstract int GoldCost { get; }
        public abstract int WoodCost { get; }
        [field: SerializeField] public Enums.Building BuildingType { get; private set; }

        [field: SerializeField] public bool Completed { get; private set; }

        public Tile Tile { get; set; }

        protected override void Awake() {
            base.Awake();
            this.Tile = this.GetComponentInParent<Tile>();
            this.BuildingNameUI = this.GetComponentInChildren<BuildingNameUI>();
        }

        public virtual void Interact(Selectable selected) { }

        public override void MouseEnter() {
            this.BuildingNameUI.Open();
        }
        public override void MouseExit() {
            this.BuildingNameUI.Close();
        }

        public void SetCompleted(bool feedback = true) {
            this.SetDurability(this.TotalDurability, feedback);
        }

        private void SetDurability(int durability, bool feedback = true) {
            this.Durability = Math.Min(durability, this.TotalDurability);
            this.BuildingNameUI.UpdateProgress((float)this.Durability / this.TotalDurability);
            if (this.Durability < this.TotalDurability)
                return;
            if (feedback) this.Tile.Feedback();
            this.Completed = true;
        }

        public void AddDurability(int durability, bool feedback = true) {
            this.SetDurability(durability + this.Durability, feedback);
        }

        public virtual bool CanBuild(ResourcesManager.ResourcesManager resourcesManager) {
            return resourcesManager.Gold >= this.GoldCost && resourcesManager.Wood >= this.WoodCost;
        }
    }
}
