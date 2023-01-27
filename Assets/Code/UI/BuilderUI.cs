using Code.Characters;
using Code.Interactable;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Building = Code.Enums.Building;

namespace Code.UI {
    public class BuilderUI : MonoBehaviour, IWithWorldCanvas {
        [field: SerializeField] private TMP_Text WindowName;
        [field: SerializeField] private Image ShopBackground;

        [field: SerializeField] private Canvas Canvas;
        [FormerlySerializedAs("ResourcesWindow")] [field: SerializeField]
        private ResourcesUI ResourcesUI;

        [field: SerializeField] private BuyUI BuyUI;
        [field: SerializeField] private Transform ButtonBox;
        [field: SerializeField] private GridLayoutGroup LayoutGroup;

        private Builder Builder;

        private LTDescr OpenTween;
        private ResourcesManager.ResourcesManager ResourcesManager;

        private void Awake() {
            this.Builder = this.GetComponentInParent<Builder>();

            this.WindowName.SetText(this.Builder.Name);

            foreach (Building building in this.Builder.Buildings) {
                BuyUI buyUI = Instantiate(this.BuyUI, this.ButtonBox);
                buyUI.Initialize(this.Builder, building);
            }

            RectTransform rect = this.ButtonBox.GetComponent<RectTransform>();
            Vector2 size = rect.sizeDelta;
            size.y = (this.LayoutGroup.cellSize.y + this.LayoutGroup.spacing.y) * this.Builder.Buildings.Count - this.LayoutGroup.spacing.y;
            rect.sizeDelta = size;

            this.Canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
            this.Canvas.gameObject.SetActive(false);
            this.Canvas.transform.localScale *= 0;

            this.ResourcesManager = GameObject.FindGameObjectWithTag("ResourcesManager").GetComponent<ResourcesManager.ResourcesManager>();
            this.ResourcesManager.ResourcesUIs.Add(this.ResourcesUI);
            this.ResourcesUI.UpdateResources(this.ResourcesManager);

            UIElements.CharacterUIElement elements = GameObject.FindGameObjectWithTag("UIElements")
                .GetComponent<UIElements>()
                .GetUIElements(this.Builder.CharacterType);
            this.ShopBackground.sprite = elements.UIElement.Ribbon;
        }

        private void Update() {
            ((IWithWorldCanvas)this).RotateCanvas(this.Canvas);
        }

        public void Open() {
            if (this.OpenTween != null) LeanTween.cancel(this.OpenTween.id);

            this.Canvas.gameObject.SetActive(true);
            float duration = 1 - this.Canvas.transform.localScale.x;
            this.OpenTween = LeanTween.scale(this.Canvas.gameObject, Vector3.one, duration * 0.25f)
                .setDelay(0.2f)
                .setEaseOutBack()
                .setOnComplete(() => this.OpenTween = null);
        }

        public void Close() {
            if (!this.Canvas.gameObject.activeSelf)
                return;

            if (this.OpenTween != null) LeanTween.cancel(this.OpenTween.id);

            float duration = this.Canvas.transform.localScale.x;
            this.OpenTween = LeanTween.scale(this.Canvas.gameObject, Vector3.zero, duration * 0.25f)
                .setEaseInBack()
                .setOnComplete(
                    () => {
                        this.Canvas.gameObject.SetActive(false);
                        this.OpenTween = null;
                    }
                );
        }
    }
}
