using System;
using System.Collections.Generic;
using Code.Interactable;
using Code.Tiles;
using TMPro;
using UnityEngine;

namespace Code.Characters {
    public class Miner : Player {

        [SerializeField] private int CarryingCapacity;
        [SerializeField] private int Carrying;
        [SerializeField] private Tile MineTile;
        [SerializeField] private long MineCooldown;
        [field: SerializeField] private TMP_Text CarryingText;
        private _Behaviour Behaviour;

        private Tile Castle;

        private Vector3 InitialScale;
        private long LastMine;
        private MouseController.MouseController MouseController;
        private ResourcesManager.ResourcesManager ResourcesManager;
        private LTDescr Tween;
        public const int GOLD_COST = 35;
        public const int WOOD_COST = 50;

        protected override void Awake() {
            base.Awake();
            this.InitialScale = this.transform.localScale;
            this.Behaviour = _Behaviour.Idle;
            this.LastMine = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            this.Castle = GameObject.FindGameObjectWithTag("Castle").GetComponent<Tile>();
            this.ResourcesManager = GameObject.FindGameObjectWithTag("ResourcesManager").GetComponent<ResourcesManager.ResourcesManager>();
            this.MouseController = GameObject.FindGameObjectWithTag("MouseController").GetComponent<MouseController.MouseController>();
        }

        protected override void Update() {
            base.Update();
            if (this.MineTile is not null && this.Behaviour == _Behaviour.Mining && this.Path.Tiles.Count == 0) {
                if (this.GroundTile.DistanceFrom(this.MineTile) == 0)
                    this.Mine();
                if (this.GroundTile.DistanceFrom(this.MineTile) == 1)
                    this.EnterMine();
            }
            if (this.Behaviour == _Behaviour.Storing && this.Path.Tiles.Count == 0 && this.GroundTile.DistanceFrom(this.Castle) == 1)
                this.StoreGold();
        }

        private void EnterMine() {
            this.MouseController.Deselect(this);

            if (this.Tween != null) LeanTween.cancel(this.Tween.id);
            this.Tween = LeanTween.scale(this.gameObject, new Vector3(.2f, .2f, .2f), 0.8f).setOnComplete(() => this.Tween = null);

            this.Path = new Path {
                Destination = this.MineTile,
                Complete = true,
                Tiles = new List<Tile> {
                    this.MineTile
                }
            };
        }

        private void Mine() {
            if (this.Carrying >= this.CarryingCapacity) {
                this.ReturnToCastle();
                return;
            }

            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (this.LastMine + this.MineCooldown > now) return;
            this.LastMine = now;
            this.Carrying++;
            this.CarryingText.SetText(this.Carrying.ToString());

            if (this.Carrying < this.CarryingCapacity)
                return;
            this.ReturnToCastle();
        }

        private void StoreGold() {
            this.ResourcesManager.AddGold(this.Carrying);
            this.Carrying = 0;
            this.CarryingText.SetText(this.Carrying.ToString());
            if (this.MineTile is null) {
                this.Behaviour = _Behaviour.Idle;
            } else {
                this.Behaviour = _Behaviour.Mining;
                this.SetPath(this.MineTile);
            }
        }
        private void ReturnToCastle() {
            if (this.Tween != null) LeanTween.cancel(this.Tween.id);
            this.Tween = LeanTween.scale(this.gameObject, this.InitialScale, 0.3f).setOnComplete(() => this.Tween = null);

            this.Behaviour = _Behaviour.Storing;
            this.SetPath(this.Castle);
        }

        public override void InteractWith(IInteractable interactable) {
            if (interactable == null) return;
            switch (interactable) {
                case Mine mine:
                    this.Behaviour = _Behaviour.Mining;
                    this.MineTile = mine.GetComponentInParent<Tile>();
                    this.MineTile.Feedback();
                    this.SetPath(this.MineTile);
                    break;
                case Interactable.Castle:
                    this.MineTile = null;
                    this.ReturnToCastle();
                    break;
            }
        }

        public override void GoToTile(Tile destination) {
            base.GoToTile(destination);
            this.Behaviour = _Behaviour.Idle;
            this.MineTile = null;
        }

        public override bool CanSummon(ResourcesManager.ResourcesManager resourcesManager) {
            return resourcesManager.Gold >= GOLD_COST && resourcesManager.Wood >= WOOD_COST;
        }

        // ReSharper disable once InconsistentNaming
        private enum _Behaviour {
            Idle,
            Mining,
            Storing
        }
    }
}
