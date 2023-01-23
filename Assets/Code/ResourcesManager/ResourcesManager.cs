using Code.Interactable;
using UnityEngine;

namespace Code.ResourcesManager {
    public class ResourcesManager : MonoBehaviour {
        public Castle Castle { private get; set; }
        [field: SerializeField] public int Wood { get; private set; }
        [field: SerializeField] public int Gold { get; private set; }

        public void AddWood(int quantity) {
            this.Wood += quantity;
            this.Castle.UpdateResources();
        }

        public void AddGold(int quantity) {
            this.Gold += quantity;
            this.Castle.UpdateResources();
        }

        public void RemoveWood(int quantity) {
            this.Wood -= quantity;
            this.Castle.UpdateResources();
        }

        public void RemoveGold(int quantity) {
            this.Gold -= quantity;
            this.Castle.UpdateResources();
        }
    }
}
