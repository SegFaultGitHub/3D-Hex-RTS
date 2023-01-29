using UnityEngine;

namespace Code.UI {
    public class UIElement : MonoBehaviour {
        [field: SerializeField] public Sprite Button { get; private set; }
        [field: SerializeField] public Sprite ButtonPressed { get; private set; }
        [field: SerializeField] public Sprite Background { get; private set; }
        [field: SerializeField] public Sprite Ribbon { get; private set; }
        [field: SerializeField] public Sprite Panel { get; private set; }
    }
}
