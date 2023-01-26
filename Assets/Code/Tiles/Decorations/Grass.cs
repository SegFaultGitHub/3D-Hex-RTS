using UnityEngine;

namespace Code.Tiles.Decorations {
    public class Grass : MonoBehaviour {
        [field: SerializeField] private Transform Decoration;

        private void Start() {
            this.transform.localEulerAngles = new Vector3(0, Random.Range(0f, 360f), 0);
            this.Decoration.localPosition = new Vector3(Random.Range(0.3f, 0.7f), 0, 0);
            float scale = Random.Range(0.7f, 1f);
            this.Decoration.localScale = new Vector3(scale, scale, scale);
            this.Decoration.localEulerAngles = new Vector3(0, Random.Range(0f, 360f), 0);
        }
    }
}
