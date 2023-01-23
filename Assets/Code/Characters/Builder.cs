using Code.Interactable;
using Code.Tiles;
using UnityEngine;

namespace Code.Characters {
    public class Builder : Player {
        private _Behaviour Behaviour;

        private MouseController.MouseController MouseController;
        private ResourcesManager.ResourcesManager ResourcesManager;
        private LTDescr Tween;
        public const int GOLD_COST = 100;
        public const int WOOD_COST = 100;

        protected override void Awake() {
            base.Awake();
            this.Behaviour = _Behaviour.Idle;
            this.ResourcesManager = GameObject.FindGameObjectWithTag("ResourcesManager").GetComponent<ResourcesManager.ResourcesManager>();
            this.MouseController = GameObject.FindGameObjectWithTag("MouseController").GetComponent<MouseController.MouseController>();
        }

        public override void InteractWith(IInteractable interactable) {
            if (interactable == null) return;
            switch (interactable) { }
        }

        public override void GoToTile(Tile destination) {
            base.GoToTile(destination);
            this.Behaviour = _Behaviour.Idle;
        }

        public override bool CanSummon(ResourcesManager.ResourcesManager resourcesManager) {
            return resourcesManager.Gold >= GOLD_COST && resourcesManager.Wood >= WOOD_COST;
        }

        // ReSharper disable once InconsistentNaming
        private enum _Behaviour {
            Idle
        }
    }
}
