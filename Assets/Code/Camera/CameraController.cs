using UnityEngine;

namespace Code.Camera {
    public class CameraController : MonoBehaviour {
        public LTDescr LookAt(Vector3 position, float duration = 0.5f) {
            Transform _transform = this.transform;
            Vector3 direction = -_transform.TransformDirection(Vector3.forward);
            direction *= 1 / direction.y * (_transform.position.y - position.y);
            return LeanTween.move(this.gameObject, position + direction, duration)
                .setEaseInOutQuad();
        }
    }
}
