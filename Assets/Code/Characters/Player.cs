using Code.Interactable;
using Code.Tiles;
using UnityEngine;

namespace Code.Characters {
    public abstract class Player : Character {

        [field: SerializeField] private int FieldOfView;
        public abstract int GoldCost { get; }
        public abstract int WoodCost { get; }

        protected override void Update() {
            Tile groundTile = this.GroundTile;
            base.Update();
            if (this.GroundTile != groundTile && this.GroundTile is not null)
                this.GroundTile.Explore(this.FieldOfView);
        }

        public virtual void InteractWith(IInteractable interactable) { }

        public bool CanSummon(ResourcesManager.ResourcesManager resourcesManager) {
            return resourcesManager.Gold >= this.GoldCost && resourcesManager.Wood >= this.WoodCost;
        }
    }
}
