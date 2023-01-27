using System.Collections.Generic;
using Code.Characters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class CharacterProgressiveBarUI : ProgressiveBarUI {
        [field: SerializeField] private List<TMP_Text> Texts;
        [field: SerializeField] private Image Background;

        public void Initialize(Character character) {
            foreach (TMP_Text text in this.Texts) {
                text.SetText(character.Name);
            }
            UIElements.CharacterUIElement elements = GameObject.FindGameObjectWithTag("UIElements")
                .GetComponent<UIElements>()
                .GetUIElements(character.CharacterType);
            this.Background.sprite = elements.UIElement.Button;
        }
    }
}
