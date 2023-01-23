using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Code.Misc {
    public class Clouds : MonoBehaviour {
        [SerializeField] private float CloudSpeed;
        [SerializeField] private float RotationSpeed;
        [field: SerializeField] private float OffsetSpeed;


        private UniversalAdditionalLightData LightData;

        private float XOffset, YOffset;

        private void Start() {
            this.LightData = this.GetComponent<Light>().GetUniversalAdditionalLightData();
        }

        private void FixedUpdate() {
            Vector2 offset = this.LightData.lightCookieOffset;
            float x = 2 * Mathf.PerlinNoise(this.XOffset, 0) - 1;
            x *= this.CloudSpeed;
            float y = 2 * Mathf.PerlinNoise(0, this.YOffset) - 1;
            y *= this.CloudSpeed;
            offset.x += x;
            offset.y += y;
            this.LightData.lightCookieOffset = offset;

            Vector2 angles = this.transform.eulerAngles;
            angles.y += this.RotationSpeed;
            // ReSharper disable once Unity.InefficientPropertyAccess
            this.transform.eulerAngles = angles;

            this.XOffset += this.OffsetSpeed;
            this.YOffset += this.OffsetSpeed;
        }
        [Serializable]
        // ReSharper disable once InconsistentNaming
        private struct _CloudSpeed {
            [field: SerializeField] public float MinSpeed { get; private set; }
            [field: SerializeField] public float MaxSpeed { get; private set; }
        }
    }
}
