using System;
using System.Collections.Generic;
using Code.Interactable;
using Code.UI;
using UnityEngine;

namespace Code.ResourcesManager {
    public class ResourcesManager : MonoBehaviour {
        public List<ResourcesWindow> ResourcesWindows { get; private set; }
        [field: SerializeField] public int Wood { get; private set; }
        [field: SerializeField] public int Gold { get; private set; }

        private void Start() {
            this.ResourcesWindows = new List<ResourcesWindow>();
        }

        public void AddWood(int quantity) {
            this.Wood += quantity;
            foreach (ResourcesWindow window in this.ResourcesWindows)
                window.UpdateResources(this);
        }

        public void AddGold(int quantity) {
            this.Gold += quantity;
            foreach (ResourcesWindow window in this.ResourcesWindows)
                window.UpdateResources(this);
        }

        public void RemoveWood(int quantity) {
            this.Wood -= quantity;
            foreach (ResourcesWindow window in this.ResourcesWindows)
                window.UpdateResources(this);
        }

        public void RemoveGold(int quantity) {
            this.Gold -= quantity;
            foreach (ResourcesWindow window in this.ResourcesWindows)
                window.UpdateResources(this);
        }
    }
}
