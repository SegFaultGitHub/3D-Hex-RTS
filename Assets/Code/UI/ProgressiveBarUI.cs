using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class ProgressiveBarUI : MonoBehaviour {
        [field: SerializeField] private RectMask2D RectMask;

        public void UpdateRatio(float ratio) {
            this.RectMask.padding = new Vector4(this.GetComponent<RectTransform>().rect.width * ratio, 0, 0, 0);
        }
    }
}
