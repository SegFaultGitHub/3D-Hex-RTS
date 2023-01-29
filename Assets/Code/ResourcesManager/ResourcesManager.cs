using System.Collections.Generic;
using Code.UI;
using UnityEngine;

namespace Code.ResourcesManager {
    public class ResourcesManager : MonoBehaviour {
        public List<ResourcesUI> ResourcesUIs { get; private set; }
        [field: SerializeField] public int Wood { get; private set; }
        [field: SerializeField] public int Gold { get; private set; }

        private void Start() {
            this.ResourcesUIs = new List<ResourcesUI>();
        }

        public void AddWood(int quantity) {
            this.Wood += quantity;
            foreach (ResourcesUI window in this.ResourcesUIs)
                window.UpdateResources(this);
        }

        public void AddGold(int quantity) {
            this.Gold += quantity;
            foreach (ResourcesUI window in this.ResourcesUIs)
                window.UpdateResources(this);
        }

        public void RemoveWood(int quantity) {
            this.Wood -= quantity;
            foreach (ResourcesUI window in this.ResourcesUIs)
                window.UpdateResources(this);
        }

        public void RemoveGold(int quantity) {
            this.Gold -= quantity;
            foreach (ResourcesUI window in this.ResourcesUIs)
                window.UpdateResources(this);
        }
    }
}
