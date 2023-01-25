using Code.Tiles;
using UnityEngine;

namespace Code.Interactable {
    public class PhantomBuilding : MonoBehaviour {
        [field: SerializeField] public Vector3 GridPosition { get; set; }
        [field: SerializeField] public Building OnCompletionBuilding { get; private set; }
        public Tile Tile { private get; set; }

        public Building Build() {
            Building building = Instantiate(this.OnCompletionBuilding, this.Tile.Objects.transform);
            Vector3 position = this.Tile.transform.position;
            position.y = this.Tile.Height;
            building.transform.position = position;
            this.Tile.Walkable = false;
            Destroy(this.gameObject);
            return building;
        }
    }
}
