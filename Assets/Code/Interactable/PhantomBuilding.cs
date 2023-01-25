using UnityEngine;

namespace Code.Interactable {
    public class PhantomBuilding : MonoBehaviour {
        [field: SerializeField] public Vector3 GridPosition { get; set; }
        [field: SerializeField] private Building OnCompletionBuilding;

        public void Build() {
            Building building = Instantiate(this.OnCompletionBuilding);
            building.transform.position = this.transform.position;
            Destroy(this.gameObject);
        }
    }
}
