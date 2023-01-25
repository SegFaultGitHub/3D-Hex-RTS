using System;
using Code.Tiles;
using Code.UI;
using UnityEngine;

namespace Code.Interactable {
    public abstract class Building : Selectable, IInteractable, IWithWorldCanvas {
        [field: SerializeField] protected Canvas MouseOverCanvas;

        private LTDescr PopupTween;
        [field: SerializeField] public bool Completed { get; private set; }
        [field: SerializeField] private ProgressiveBarUI ProgressBar;
        [field: SerializeField] private int Durability;
        [field: SerializeField] private int TotalDurability;

        protected void Start() {
            this.MouseOverCanvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
            this.MouseOverCanvas.gameObject.SetActive(false);
            this.MouseOverCanvas.transform.localScale *= 0;
        }

        protected void Update() {
            ((IWithWorldCanvas)this).RotateCanvas(this.MouseOverCanvas);
        }

        public abstract void Interact(Selectable selected);

        public override void MouseEnter() {
            this.ShowUI();
        }
        public override void MouseExit() {
            this.HideUI();
        }

        private void HideUI() {
            if (!this.MouseOverCanvas.gameObject.activeSelf)
                return;

            if (this.PopupTween != null) LeanTween.cancel(this.PopupTween.id);

            float duration = this.MouseOverCanvas.transform.localScale.x;
            this.PopupTween = LeanTween.scale(this.MouseOverCanvas.gameObject, Vector3.zero, duration * 0.25f)
                .setEaseInBack()
                .setOnComplete(
                    () => {
                        this.MouseOverCanvas.gameObject.SetActive(false);
                        this.PopupTween = null;
                    }
                );
        }

        private void ShowUI() {
            if (this.PopupTween != null) LeanTween.cancel(this.PopupTween.id);

            this.MouseOverCanvas.gameObject.SetActive(true);
            float duration = 1 - this.MouseOverCanvas.transform.localScale.x;
            this.PopupTween = LeanTween.scale(this.MouseOverCanvas.gameObject, Vector3.one, duration * 0.25f)
                .setEaseOutBack()
                .setOnComplete(() => this.PopupTween = null);
        }

        public void SetCompleted() {
            this.SetDurability(this.TotalDurability);
        }

        private void SetDurability(int durability) {
            this.Durability = Math.Min(durability, this.TotalDurability);
            float ratio = (float)this.Durability / this.TotalDurability;
            this.ProgressBar.UpdateRatio(ratio);
            if (this.Durability < this.TotalDurability)
                return;
            this.GetComponentInParent<Tile>().Feedback();
            this.Completed = true;
        }

        public void AddDurability(int durability) {
            this.SetDurability(durability + this.Durability);
        }

        public virtual bool CanBuild(ResourcesManager.ResourcesManager resourcesManager) {
            return false;
        }
    }
}
