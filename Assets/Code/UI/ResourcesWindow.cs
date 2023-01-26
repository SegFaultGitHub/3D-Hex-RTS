using TMPro;
using UnityEngine;

namespace Code.UI {
    public class ResourcesWindow : MonoBehaviour {
        [field: SerializeField] private TMP_Text GoldQuantityText, WoodQuantityText;

        public void UpdateResources(ResourcesManager.ResourcesManager resourcesManager) {
            this.GoldQuantityText.text = resourcesManager.Gold.ToString();
            this.WoodQuantityText.text = resourcesManager.Wood.ToString();
        }
    }
}
