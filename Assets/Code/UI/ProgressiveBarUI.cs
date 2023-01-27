using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class ProgressiveBarUI : MonoBehaviour {
        [field: SerializeField] private RectMask2D RectMask;
        private float Width;

        public void UpdateRatio(float ratio) {
            if (this.Width == 0) this.Width = this.GetComponent<RectTransform>().rect.width;
            this.RectMask.padding = new Vector4(this.Width * ratio, 0, 0, 0);
        }
    }
}
