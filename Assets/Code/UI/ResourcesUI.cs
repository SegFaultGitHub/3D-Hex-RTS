using Code.Characters;
using Code.Interactable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class ResourcesUI : MonoBehaviour {
        [field: SerializeField] private TMP_Text GoldQuantityText, WoodQuantityText;
        [field: SerializeField] private Image Background;

        private void Awake() {
            Building building = this.GetComponentInParent<Building>();
            Character character = this.GetComponentInParent<Character>();
            if (building != null) {
                UIElements.BuildingUIElement elements = GameObject.FindGameObjectWithTag("UIElements")
                    .GetComponent<UIElements>()
                    .GetUIElements(building.BuildingType);
                this.Background.sprite = elements.UIElement.Panel;
            } else if (character != null) {
                UIElements.CharacterUIElement elements = GameObject.FindGameObjectWithTag("UIElements")
                    .GetComponent<UIElements>()
                    .GetUIElements(character.CharacterType);
                this.Background.sprite = elements.UIElement.Panel;
            }
        }

        public void UpdateResources(ResourcesManager.ResourcesManager resourcesManager) {
            this.GoldQuantityText.text = resourcesManager.Gold.ToString();
            this.WoodQuantityText.text = resourcesManager.Wood.ToString();
        }
    }
}
