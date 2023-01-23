using UnityEngine;

namespace Code.Interactable {
    public interface IWithWorldCanvas {
        public void RotateCanvas(Canvas canvas) {
            canvas.transform.eulerAngles = canvas.worldCamera.transform.eulerAngles;
        }
    }
}
