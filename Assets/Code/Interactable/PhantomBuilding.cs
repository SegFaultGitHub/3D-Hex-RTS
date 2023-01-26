using System.Reflection;
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
            Transform t = building.transform;
            t.position = position;
            t.eulerAngles = this.transform.eulerAngles;
            this.Tile.Walkable = false;

            int goldCost = (int)this.OnCompletionBuilding.GetType().GetField("GOLD_COST", BindingFlags.Public | BindingFlags.Static)!
                .GetValue(null);
            int woodCost = (int)this.OnCompletionBuilding.GetType().GetField("WOOD_COST", BindingFlags.Public | BindingFlags.Static)!
                .GetValue(null);

            ResourcesManager.ResourcesManager resourcesManager =
                GameObject.FindGameObjectWithTag("ResourcesManager").GetComponent<ResourcesManager.ResourcesManager>();
            resourcesManager.RemoveGold(goldCost);
            resourcesManager.RemoveWood(woodCost);

            Destroy(this.gameObject);
            return building;
        }
    }
}
