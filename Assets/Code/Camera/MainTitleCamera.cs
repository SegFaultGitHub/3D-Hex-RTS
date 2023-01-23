using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Code.Camera {
    public class MainTitleCamera : MonoBehaviour {
        [SerializeField] private Vector3 Offset;
        [SerializeField] private float CameraSpeed;

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private void Update() {
            Vector3 angles = this.transform.eulerAngles;
            angles.y += this.CameraSpeed;
            this.transform.eulerAngles = angles;
            Vector3 rotatedOffset = Quaternion.Euler(0, this.transform.eulerAngles.y, 0) * this.Offset;
            this.transform.position = rotatedOffset;
        }
    }
}
