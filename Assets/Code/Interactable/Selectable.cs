using UnityEngine;

namespace Code.Interactable {
    [RequireComponent(typeof(Outline))]
    public abstract class Selectable : MonoBehaviour, IMouseOver {
        private Outline Outline;
        private LTDescr Tween;

        protected virtual void Awake() {
            this.Outline = this.GetComponent<Outline>();
            this.Outline.enabled = false;
        }

        public abstract void MouseEnter();
        public abstract void MouseExit();

        public void Select() {
            if (this.Tween != null)
                LeanTween.cancel(this.Tween.id);

            this.Tween = LeanTween.value(this.gameObject, this.Outline.OutlineColor, new Color(1f, 1f, 1f, 1f), 0.2f)
                .setOnStart(() => this.Outline.enabled = true)
                .setOnUpdate(
                    color => {
                        this.Outline.OutlineColor = color;
                    }
                )
                .setOnComplete(() => this.Tween = null);
        }

        public void Deselect() {
            if (this.Tween != null)
                LeanTween.cancel(this.Tween.id);

            this.Tween = LeanTween.value(this.gameObject, this.Outline.OutlineColor, new Color(1f, 1f, 1f, 0f), 0.2f)
                .setOnUpdate(
                    color => {
                        this.Outline.OutlineColor = color;
                    }
                )
                .setOnComplete(
                    () => {
                        this.Outline.enabled = false;
                        this.Tween = null;
                    }
                );
        }
    }
}
