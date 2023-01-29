using Code.Extensions;
using UnityEngine;

namespace Code.Interactable {
    public struct TreeConsumption {
        public int Quantity;
        public bool Depleted;
    }

    public class Trees : Building {
        [field: SerializeField] private int MinQuantity, MaxQuantity;
        [field: SerializeField] private GameObject TreesModel;

        private int InitialQuantity;
        private LTDescr Tween;
        public override string Name => "Trees";
        public override int GoldCost => -1;
        public override int WoodCost => -1;
        [field: SerializeField] private int Quantity { get; set; }

        protected override void Awake() {
            base.Awake();
            this.OnEndOfFrame(() => this.SetCompleted(false));
            this.Quantity = Random.Range(this.MinQuantity, this.MaxQuantity);
            this.InitialQuantity = this.Quantity;
        }

        private void Shake() {
            if (this.Tween != null) return;
            this.Tween = LeanTween.rotateZ(this.TreesModel, 5, .1f)
                .setOnComplete(
                    () => {
                        LeanTween.rotateZ(this.TreesModel, -5, .2f)
                            .setOnComplete(
                                () => {
                                    LeanTween.rotateZ(this.TreesModel, 0, .1f).setOnComplete(() => this.Tween = null);
                                }
                            );
                    }
                );
        }

        public TreeConsumption Consume(int quantity) {
            this.Shake();
            quantity = Mathf.Min(this.Quantity, quantity);
            this.Quantity -= quantity;
            this.BuildingNameUI.UpdateProgress(this.Quantity / (float)this.InitialQuantity);

            if (this.Quantity > 0)
                return new TreeConsumption {
                    Quantity = quantity,
                    Depleted = false
                };
            this.Tile.Walkable = true;
            Destroy(this.gameObject);
            return new TreeConsumption {
                Quantity = quantity,
                Depleted = true
            };
        }

        public override bool CanBuild(ResourcesManager.ResourcesManager resourcesManager) {
            return false;
        }
    }
}
