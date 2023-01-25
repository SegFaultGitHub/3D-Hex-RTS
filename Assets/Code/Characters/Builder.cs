using System;
using System.Reflection;
using Code.Interactable;
using Code.Tiles;
using UnityEngine;

namespace Code.Characters {
    public class Builder : Player {
        private static readonly int ATTACK = Animator.StringToHash("Attack");
        [field: SerializeField] private Tile BuildingTile;
        [field: SerializeField] private long BuildCooldown;
        [field: SerializeField] private int BuildPower = 10;
        private long LastBuild;
        private _Behaviour Behaviour;
        private ResourcesManager.ResourcesManager ResourcesManager;
        private MouseController.MouseController MouseController;

        [field: SerializeField] private Canvas ShopCanvas;
        private LTDescr Tween;

        [field: SerializeField] private PhantomBuilding Barracks;

        public const int GOLD_COST = 100;
        public const int WOOD_COST = 100;

        protected override void Awake() {
            base.Awake();
            this.Behaviour = _Behaviour.Idle;
            this.LastBuild = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            this.ShopCanvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
            this.ShopCanvas.gameObject.SetActive(false);
            this.ShopCanvas.transform.localScale *= 0;
            this.ResourcesManager = GameObject.FindGameObjectWithTag("ResourcesManager").GetComponent<ResourcesManager.ResourcesManager>();
            this.MouseController = GameObject.FindGameObjectWithTag("MouseController").GetComponent<MouseController.MouseController>();
        }

        protected override void Update() {
            base.Update();
            ((IWithWorldCanvas)this).RotateCanvas(this.ShopCanvas);
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

            Building building = this.BuildingTile.GetComponentInChildren<Building>();
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
                case Building { Completed: false } building:
                    this.Behaviour = _Behaviour.Building;
                    this.BuildingTile = building.GetComponentInParent<Tile>();
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

        public override bool CanSummon(ResourcesManager.ResourcesManager resourcesManager) {
            return resourcesManager.Gold >= GOLD_COST && resourcesManager.Wood >= WOOD_COST;
        }

        public void CreatePhantomBarracks() {
            this.CreatePhantomBuilding(this.Barracks);
        }

        private void CreatePhantomBuilding(PhantomBuilding phantom) {
            if (!phantom.OnCompletionBuilding.CanBuild(this.ResourcesManager)) return;

            int goldCost = (int)phantom.OnCompletionBuilding.GetType().GetField("GOLD_COST", BindingFlags.Public | BindingFlags.Static)!.GetValue(null);
            int woodCost = (int)phantom.OnCompletionBuilding.GetType().GetField("WOOD_COST", BindingFlags.Public | BindingFlags.Static)!.GetValue(null);

            this.ResourcesManager.RemoveGold(goldCost);
            this.ResourcesManager.RemoveWood(woodCost);

            this.MouseController.CreatePhantomBuilding(this, phantom);
            this.MouseController.Deselect(this);
        }

        public override void OnSelect() {
            this.MouseExit();

            if (this.Tween != null) LeanTween.cancel(this.Tween.id);

            this.ShopCanvas.gameObject.SetActive(true);
            float duration = 1 - this.ShopCanvas.transform.localScale.x;
            this.Tween = LeanTween.scale(this.ShopCanvas.gameObject, Vector3.one, duration * 0.25f)
                .setDelay(0.2f)
                .setEaseOutBack()
                .setOnComplete(() => this.Tween = null);
        }

        public override void OnDeselect() {
            base.MouseExit();
            if (!this.ShopCanvas.gameObject.activeSelf)
                return;

            if (this.Tween != null) LeanTween.cancel(this.Tween.id);

            float duration = this.ShopCanvas.transform.localScale.x;
            this.Tween = LeanTween.scale(this.ShopCanvas.gameObject, Vector3.zero, duration * 0.25f)
                .setEaseInBack()
                .setOnComplete(
                    () => {
                        this.ShopCanvas.gameObject.SetActive(false);
                        this.Tween = null;
                    }
                );
        }

        // ReSharper disable once InconsistentNaming
        private enum _Behaviour {
            Idle,
            Building
        }
    }
}
