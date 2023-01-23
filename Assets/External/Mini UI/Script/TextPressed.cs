using UnityEngine;
using UnityEngine.EventSystems;

public class TextPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    [SerializeField] private RectTransform gameObject;
    public float UnitsToMove;
    private Vector2 vector2;

    private void Start() => this.vector2 = this.gameObject.GetComponent<RectTransform>().anchoredPosition;

    public void OnPointerDown(PointerEventData eventData) =>
        this.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, this.UnitsToMove);

    public void OnPointerUp(PointerEventData eventData) =>
        this.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(this.vector2.x, this.vector2.y);

    public void moveDown() { }
}
