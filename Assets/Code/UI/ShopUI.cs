using Code.Enums;
using Code.Interactable;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Code.UI {
    public class ShopUI : MonoBehaviour, IWithWorldCanvas {
        [field: SerializeField] private TMP_Text WindowName;
        [field: SerializeField] private Image ShopBackground;
        [field: SerializeField] private Image QueueBackground;

        [field: SerializeField] private Canvas Canvas;
        [FormerlySerializedAs("ResourcesWindow")] [field: SerializeField]
        private ResourcesUI ResourcesUI;

        [field: SerializeField] private CharacterProgressiveBarUI CharacterProgressiveBarUI;

        [field: SerializeField] private BuyUI BuyUI;
        [field: SerializeField] private Transform ButtonBox;
        [field: SerializeField] private GridLayoutGroup LayoutGroup;

        [field: SerializeField] private GridLayoutGroup QueueGridLayoutGroup;
        [field: SerializeField] private GameObject QueueWindow;
        [field: SerializeField] private RectTransform QueueTransform;
        private LTDescr OpenTween;
        private LTDescr QueueTween;
        private bool QueueWindowOpened;
        private ResourcesManager.ResourcesManager ResourcesManager;

        private Shop Shop;

        private void Awake() {
            this.Shop = this.GetComponentInParent<Shop>();

            this.WindowName.SetText(this.Shop.Name);

            foreach (Character character in this.Shop.Characters) {
                BuyUI buyUI = Instantiate(this.BuyUI, this.ButtonBox);
                buyUI.Initialize(this.Shop, character);
            }

            RectTransform rect = this.ButtonBox.GetComponent<RectTransform>();
            Vector2 size = rect.sizeDelta;
            size.y = (this.LayoutGroup.cellSize.y + this.LayoutGroup.spacing.y) * this.Shop.Characters.Count - this.LayoutGroup.spacing.y;
            rect.sizeDelta = size;

            this.QueueWindow.transform.localScale *= 0;

            this.Canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
            this.Canvas.gameObject.SetActive(false);
            this.Canvas.transform.localScale *= 0;

            this.ResourcesManager = GameObject.FindGameObjectWithTag("ResourcesManager").GetComponent<ResourcesManager.ResourcesManager>();
            this.ResourcesManager.ResourcesUIs.Add(this.ResourcesUI);
            this.ResourcesUI.UpdateResources(this.ResourcesManager);

            UIElements.BuildingUIElement elements = GameObject.FindGameObjectWithTag("UIElements")
                .GetComponent<UIElements>()
                .GetUIElements(this.Shop.BuildingType);
            this.ShopBackground.sprite = elements.UIElement.Ribbon;
            this.QueueBackground.sprite = elements.UIElement.Panel;
        }

        private void Update() {
            ((IWithWorldCanvas)this).RotateCanvas(this.Canvas);
        }

        public void UpdateQueueUI(int count) {
            if (this.QueueTween != null) LeanTween.cancel(this.QueueTween.id);

            if (count == 0 && this.QueueWindowOpened) {
                this.QueueWindowOpened = false;
                this.QueueTween = LeanTween.scale(this.QueueWindow, Vector3.zero, 0.2f)
                    .setEaseInBack()
                    .setOnComplete(() => this.QueueTween = null);
            } else if (count != 0 && !this.QueueWindowOpened) {
                this.QueueWindowOpened = true;
                this.QueueTween = LeanTween.scale(this.QueueWindow, Vector3.one, 0.2f)
                    .setEaseOutBack()
                    .setOnComplete(() => this.QueueTween = null);
            }

            Vector2 size = this.QueueTransform.sizeDelta;
            size.y = (this.QueueGridLayoutGroup.cellSize.y + this.QueueGridLayoutGroup.spacing.y) * count
                     - this.QueueGridLayoutGroup.spacing.y;
            this.QueueTransform.sizeDelta = size;
        }

        public CharacterProgressiveBarUI AddToQueue() {
            return Instantiate(this.CharacterProgressiveBarUI, this.QueueTransform.transform);
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
