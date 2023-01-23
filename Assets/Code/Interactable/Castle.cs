using System.Reflection;
using Code.Characters;
using Code.Tiles;
using TMPro;
using UnityEngine;

namespace Code.Interactable {
    public class Castle : Building {
        [field: SerializeField] private Lumberjack LumberjackPrefab;
        [field: SerializeField] private Miner MinerPrefab;
        [field: SerializeField] private Builder BuilderPrefab;

        [field: SerializeField] private Canvas Canvas;

        [field: SerializeField] private TMP_Text GoldQuantityText, WoodQuantityText;
        private ResourcesManager.ResourcesManager ResourcesManager;

        private Tile Tile;
        private LTDescr Tween;

        private new void Start() {
            base.Start();
            this.Tile = this.GetComponentInParent<Tile>();
            this.Canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
            this.Canvas.gameObject.SetActive(false);
            this.Canvas.transform.localScale *= 0;

            this.ResourcesManager = GameObject.FindGameObjectWithTag("ResourcesManager").GetComponent<ResourcesManager.ResourcesManager>();
            this.ResourcesManager.Castle = this;
            this.UpdateResources();
        }

        protected new void Update() {
            base.Update();
            ((IWithWorldCanvas)this).RotateCanvas(this.Canvas);
        }

        public override void Interact(Selectable selected) {
            this.MouseExit();

            if (this.Tween != null) LeanTween.cancel(this.Tween.id);

            this.Canvas.gameObject.SetActive(true);
            float duration = 1 - this.Canvas.transform.localScale.x;
            this.Tween = LeanTween.scale(this.Canvas.gameObject, Vector3.one, duration * 0.25f)
                .setDelay(0.2f)
                .setEaseOutBack()
                .setOnComplete(() => this.Tween = null);
        }

        public override void MouseExit() {
            base.MouseExit();
            if (!this.Canvas.gameObject.activeSelf)
                return;

            if (this.Tween != null) LeanTween.cancel(this.Tween.id);

            float duration = this.Canvas.transform.localScale.x;
            this.Tween = LeanTween.scale(this.Canvas.gameObject, Vector3.zero, duration * 0.25f)
                .setEaseInBack()
                .setOnComplete(
                    () => {
                        this.Canvas.gameObject.SetActive(false);
                        this.Tween = null;
                    }
                );
        }

        public void SpawnLumberjack() {
            this.SpawnPlayer(this.LumberjackPrefab);
        }

        public void SpawnMiner() {
            this.SpawnPlayer(this.MinerPrefab);
        }

        public void SpawnBuilder() {
            this.SpawnPlayer(this.BuilderPrefab);
        }

        private void SpawnPlayer(Player prefab) {
            if (!prefab.CanSummon(this.ResourcesManager)) return;

            int goldCost = (int) prefab.GetType().GetField("GOLD_COST", BindingFlags.Public | BindingFlags.Static)!.GetValue(null);
            int woodCost = (int) prefab.GetType().GetField("WOOD_COST", BindingFlags.Public | BindingFlags.Static)!.GetValue(null);

            this.ResourcesManager.RemoveGold(goldCost);
            this.ResourcesManager.RemoveWood(woodCost);

            Player player = Instantiate(prefab);
            Vector3 position = this.Tile.transform.position;
            position.y = this.Tile.Height;
            player.SetPosition(position);
            player.SetGroundTile(this.Tile);
            player.GoToTile(this.Tile.GetRandomNeighbour(player.StepOffset, 2, 3));
        }

        public void UpdateResources() {
            this.GoldQuantityText.text = this.ResourcesManager.Gold.ToString();
            this.WoodQuantityText.text = this.ResourcesManager.Wood.ToString();
        }
    }
}
