using System;
using System.Collections.Generic;
using System.Reflection;
using Code.Characters;
using Code.Tiles;
using Code.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Interactable {
    public class Castle : Building {
        [Serializable]
        private class PlayerCreation {
            public Player Player;
            public long CreationDuration;
            public long SpawnStart;
            public ProgressiveBarUI BarUI;
        }

        [field: SerializeField] private Lumberjack LumberjackPrefab;
        [field: SerializeField] private Miner MinerPrefab;
        [field: SerializeField] private Builder BuilderPrefab;

        [field: SerializeField] private Canvas Canvas;

        [field: SerializeField] private TMP_Text GoldQuantityText, WoodQuantityText;
        private ResourcesManager.ResourcesManager ResourcesManager;

        private Tile Tile;
        private LTDescr Tween;

        [Space, Header("Queue")]
        [field: SerializeField] private List<PlayerCreation> PlayerCreationQueue = new();
        [field: SerializeField] private GridLayoutGroup QueueGridLayoutGroup;
        [field: SerializeField] private GameObject QueueWindow;
        [field: SerializeField] private RectTransform QueueTransform;
        private bool QueueWindowOpened;
        private LTDescr QueueTween;


        private new void Start() {
            base.Start();
            this.Tile = this.GetComponentInParent<Tile>();
            this.Canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
            this.Canvas.gameObject.SetActive(false);
            this.Canvas.transform.localScale *= 0;

            this.ResourcesManager = GameObject.FindGameObjectWithTag("ResourcesManager").GetComponent<ResourcesManager.ResourcesManager>();
            this.ResourcesManager.Castle = this;
            this.UpdateResources();

            this.QueueWindowOpened = false;
            this.QueueWindow.transform.localScale *= 0;
        }

        protected new void Update() {
            base.Update();
            ((IWithWorldCanvas)this).RotateCanvas(this.Canvas);

            this.UpdateQueue();
        }

        private void UpdateQueue() {
            if (this.PlayerCreationQueue.Count == 0)
                return;

            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            PlayerCreation current = this.PlayerCreationQueue[0];
            if (current.SpawnStart == -1) current.SpawnStart = now;
            float ratio = (now - current.SpawnStart) / (float)current.CreationDuration;
            if (ratio >= 1) {
                Destroy(current.BarUI.gameObject);
                this.PlayerCreationQueue.Remove(current);
                this.UpdateQueueUI();
                this.SpawnPlayer(current.Player);
            } else {
                current.BarUI.UpdateRatio(ratio);
            }
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
            this.AddToQueue(this.LumberjackPrefab);
        }

        public void SpawnMiner() {
            this.AddToQueue(this.MinerPrefab);
        }

        public void SpawnBuilder() {
            this.AddToQueue(this.BuilderPrefab);
        }

        private void AddToQueue(Player player) {
            if (!player.CanSummon(this.ResourcesManager)) return;

            int goldCost = (int)player.GetType().GetField("GOLD_COST", BindingFlags.Public | BindingFlags.Static)!.GetValue(null);
            int woodCost = (int)player.GetType().GetField("WOOD_COST", BindingFlags.Public | BindingFlags.Static)!.GetValue(null);

            this.ResourcesManager.RemoveGold(goldCost);
            this.ResourcesManager.RemoveWood(woodCost);
            ProgressiveBarUI bar = Instantiate(player.UICreationPrefab, this.QueueTransform.transform);

            this.PlayerCreationQueue.Add(
                new PlayerCreation {
                    Player = player,
                    CreationDuration = player.CreationDuration,
                    SpawnStart = -1,
                    BarUI = bar
                }
            );
            this.UpdateQueueUI();
        }

        private void SpawnPlayer(Player prefab) {
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

        private void UpdateQueueUI() {
            if (this.QueueTween != null) LeanTween.cancel(this.QueueTween.id);

            if (this.PlayerCreationQueue.Count == 0 && this.QueueWindowOpened) {
                this.QueueWindowOpened = false;
                this.QueueTween = LeanTween.scale(this.QueueWindow, Vector3.zero, 0.2f)
                    .setEaseInBack()
                    .setOnComplete(() => this.QueueTween = null);
            } else if (this.PlayerCreationQueue.Count != 0 && !this.QueueWindowOpened) {
                this.QueueWindowOpened = true;
                this.QueueTween = LeanTween.scale(this.QueueWindow, Vector3.one, 0.2f)
                    .setEaseOutBack()
                    .setOnComplete(() => this.QueueTween = null);
            }

            Vector2 size = this.QueueTransform.sizeDelta;
            size.y = (this.QueueGridLayoutGroup.cellSize.y + this.QueueGridLayoutGroup.spacing.y) * this.PlayerCreationQueue.Count
                     - this.QueueGridLayoutGroup.spacing.y;
            this.QueueTransform.sizeDelta = size;
        }
    }
}
