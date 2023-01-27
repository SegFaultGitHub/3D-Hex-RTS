using Code.Characters;
using Code.Interactable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class CharacterNameUI : MonoBehaviour, IWithWorldCanvas {
        [field: SerializeField] private Canvas Canvas;

        [field: SerializeField] private TMP_Text Text;
        [field: SerializeField] private Image Background;

        private LTDescr Tween;

        private void Awake() {
            this.Canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
            this.Canvas.gameObject.SetActive(false);
            this.Canvas.transform.localScale *= 0;


            Character character = this.GetComponentInParent<Character>();
            this.Text.SetText(character.Name);
            UIElements.CharacterUIElement elements = GameObject.FindGameObjectWithTag("UIElements")
                .GetComponent<UIElements>()
                .GetUIElements(character.CharacterType);
            this.Background.sprite = elements.UIElement.Background;
        }

        private void Update() {
            ((IWithWorldCanvas)this).RotateCanvas(this.Canvas);
        }

        public void Open() {
            if (this.Tween != null) LeanTween.cancel(this.Tween.id);

            this.Canvas.gameObject.SetActive(true);
            float duration = 1 - this.Canvas.transform.localScale.x;
            this.Tween = LeanTween.scale(this.Canvas.gameObject, Vector3.one, duration * 0.25f)
                .setEaseOutBack()
                .setOnComplete(() => this.Tween = null);
        }

        public void Close() {
            if (!this.Canvas.gameObject.activeSelf)
                return;

            if (this.Tween != null) LeanTween.cancel(this.Tween.id);

            float duration = this.Canvas.transform.localScale.x;
            this.Tween = LeanTween.scale(this.Canvas.gameObject, Vector3.zero, duration * 0.25f)
                .setEaseInBack()
                .setOnComplete(
                    () => {
                        this.Canvas.gameObject.SetActive(false);
                        this.Tween = null;
                    }
                );
        }
    }
}
