using System;
using Code.Tiles;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.Interactable {
    public struct TreeConsumption {
        public int Quantity;
        public bool Depleted;
    }

    public class Trees : Building {
        [field: SerializeField] private int MinQuantity, MaxQuantity;
        [field: SerializeField] private RectTransform QuantityBackground;
        [field: SerializeField] private RectMask2D RectMask;
        private int InitialQuantity;
        private LTDescr Tween;

        [Space, Header("UI")]
        private float UIWidth;
        [field: SerializeField] private int Quantity { get; set; }


        private new void Start() {
            base.Start();
            this.Quantity = Random.Range(this.MinQuantity, this.MaxQuantity);
            this.InitialQuantity = this.Quantity;
            this.UIWidth = this.MouseOverCanvas.transform.GetComponent<RectTransform>().rect.width;
        }

        public override void Interact(Selectable selected) {
            throw new NotImplementedException();
        }

        private void Shake() {
            if (this.Tween != null) return;
            this.Tween = LeanTween.rotateZ(this.gameObject, 5, .1f)
                .setOnComplete(
                    () => {
                        LeanTween.rotateZ(this.gameObject, -5, .2f)
                            .setOnComplete(
                                () => {
                                    LeanTween.rotateZ(this.gameObject, 0, .1f).setOnComplete(() => this.Tween = null);
                                }
                            );
                    }
                );
        }

        public TreeConsumption Consume(int quantity) {
            this.Shake();
            quantity = Mathf.Min(this.Quantity, quantity);
            this.Quantity -= quantity;
            this.UpdateQuantityUI();

            if (this.Quantity > 0)
                return new TreeConsumption {
                    Quantity = quantity,
                    Depleted = false
                };
            Tile tile = this.GetComponentInParent<Tile>();
            tile.Walkable = true;
            Destroy(this.gameObject);
            return new TreeConsumption {
                Quantity = quantity,
                Depleted = true
            };
        }

        private void UpdateQuantityUI() {
            float ratio = this.Quantity / (float)this.InitialQuantity;
            Vector2 size = this.QuantityBackground.sizeDelta;
            size.x = -this.UIWidth * (1 - ratio);
            this.QuantityBackground.sizeDelta = size;
            this.RectMask.padding = new Vector4(0, 0, this.UIWidth * (1 - ratio), 0);
        }
    }
}
