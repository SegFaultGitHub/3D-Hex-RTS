using System;
using System.Collections.Generic;
using Code.Characters;
using Code.UI;
using UnityEngine;
using Character = Code.Enums.Character;

namespace Code.Interactable {
    public abstract class Shop : Building {

        private readonly List<PlayerCreation> PlayerCreationQueue = new();
        private ResourcesManager.ResourcesManager ResourcesManager;
        private ShopUI ShopUI;

        private LTDescr Tween;
        [field: SerializeField] public List<Character> Characters { get; private set; }

        private new void Awake() {
            base.Awake();
            this.ResourcesManager = GameObject.FindGameObjectWithTag("ResourcesManager").GetComponent<ResourcesManager.ResourcesManager>();
            this.ShopUI = this.GetComponentInChildren<ShopUI>();
        }

        protected void Update() {
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
                this.ShopUI.UpdateQueueUI(this.PlayerCreationQueue.Count);
                this.SpawnPlayer(current.Player);
            } else {
                current.BarUI.UpdateRatio(ratio);
            }
        }

        public override void Interact(Selectable selected) {
            if (!this.Completed) return;
            this.MouseExit();

            this.ShopUI.Open();
        }

        public override void MouseExit() {
            base.MouseExit();

            this.ShopUI.Close();
        }

        public void AddToQueue(Player player) {
            if (!player.CanSummon(this.ResourcesManager)) return;

            this.ResourcesManager.RemoveGold(player.GoldCost);
            this.ResourcesManager.RemoveWood(player.WoodCost);
            CharacterProgressiveBarUI bar = this.ShopUI.AddToQueue();
            bar.Initialize(player);

            this.PlayerCreationQueue.Add(
                new PlayerCreation {
                    Player = player,
                    CreationDuration = player.CreationDuration,
                    SpawnStart = -1,
                    BarUI = bar
                }
            );
            this.ShopUI.UpdateQueueUI(this.PlayerCreationQueue.Count);
        }

        public void SpawnPlayer(Player prefab) {
            Player player = Instantiate(prefab);
            Vector3 position = this.Tile.transform.position;
            position.y = this.Tile.Height;
            player.SetPosition(position);
            player.SetGroundTile(this.Tile);
            player.GoToTile(this.Tile.GetRandomNeighbour(player.StepOffset, 2, 3));
        }

        [Serializable]
        private class PlayerCreation {
            public Player Player;
            public long CreationDuration;
            public long SpawnStart;
            public ProgressiveBarUI BarUI;
        }
    }
}
