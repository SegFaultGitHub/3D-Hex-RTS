using UnityEngine;

namespace Code.Tiles {
    public class Grass : Tile {
        [field: SerializeField] private Decorations.Grass Decoration;

        protected override void Awake() {
            base.Awake();
            if (Utils.Utils.Rate(0.9f)) return;

            Instantiate(this.Decoration, this.Objects.transform);
        }
    }
}
