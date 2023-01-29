using System.Collections.Generic;
using Code.Interactable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class BuildingNameUI : MonoBehaviour, IWithWorldCanvas {

        [field: SerializeField] private Canvas Canvas;

        [field: SerializeField] private List<TMP_Text> Texts;
        [field: SerializeField] private Image Background;
        [field: SerializeField] private ProgressiveBarUI ProgressBar;

        private LTDescr Tween;

        private void Awake() {
            this.Canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
            this.Canvas.gameObject.SetActive(false);
            this.Canvas.transform.localScale *= 0;


            Building building = this.GetComponentInParent<Building>();
            foreach (TMP_Text text in this.Texts) {
                text.SetText(building.Name);
            }
            UIElements.BuildingUIElement elements = GameObject.FindGameObjectWithTag("UIElements")
                .GetComponent<UIElements>()
                .GetUIElements(building.BuildingType);
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

        public void UpdateProgress(float ratio) {
            this.ProgressBar.UpdateRatio(ratio);
        }
    }
}
