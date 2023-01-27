using Code.Characters;
using Code.Interactable;
using Code.ResourcesManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Building = Code.Enums.Building;
using Character = Code.Enums.Character;

namespace Code.UI {
    public class BuyUI : MonoBehaviour {
        [field: SerializeField] private Button Button;
        [field: SerializeField] private TMP_Text Text;
        [field: SerializeField] private Cost Cost;

        public void Initialize(Shop shop, Character character) {
            UIElements.CharacterUIElement elements =
                GameObject.FindGameObjectWithTag("UIElements").GetComponent<UIElements>().GetUIElements(character);

            this.Cost.SetText(elements.Prefab);
            this.Text.SetText(elements.Prefab!.Name);

            this.Button.GetComponent<Image>().sprite = elements.UIElement.Button;
            SpriteState spriteState = this.Button.spriteState;
            spriteState.pressedSprite = elements.UIElement.ButtonPressed;
            this.Button.spriteState = spriteState;

            this.Button.onClick.AddListener(() => shop.AddToQueue(elements.Prefab));
        }

        public void Initialize(Builder builder, Building building) {
            UIElements.BuildingUIElement elements =
                GameObject.FindGameObjectWithTag("UIElements").GetComponent<UIElements>().GetUIElements(building);

            this.Cost.SetText(elements.PhantomPrefab.OnCompletionBuilding);
            this.Text.SetText(elements.PhantomPrefab.OnCompletionBuilding.Name);

            this.Button.GetComponent<Image>().sprite = elements.UIElement.Button;
            SpriteState spriteState = this.Button.spriteState;
            spriteState.pressedSprite = elements.UIElement.ButtonPressed;
            this.Button.spriteState = spriteState;

            this.Button.onClick.AddListener(() => builder.CreatePhantomBuilding(elements.PhantomPrefab));
        }
    }
}
