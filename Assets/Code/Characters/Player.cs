using Code.Interactable;
using Code.Tiles;
using UnityEngine;

namespace Code.Characters {
    public abstract class Player : Character {
        [field: SerializeField] private int FieldOfView;

        protected override void Update() {
            Tile groundTile = this.GroundTile;
            base.Update();
            if (this.GroundTile != groundTile && this.GroundTile is not null)
                this.GroundTile.Explore(this.FieldOfView);
        }

        public virtual void InteractWith(IInteractable interactable) { }

        public abstract bool CanSummon(ResourcesManager.ResourcesManager resourcesManager);
    }
}
