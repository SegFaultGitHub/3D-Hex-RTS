using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Map : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler {

    [SerializeField] private GameObject MapObj;
    [SerializeField] private RectTransform Viewport_Dimensions;
    private Vector2 Half_Map_Background_Dimensions;
    private Vector2 Half_Viewport_Dimensions;
    private float HorizontalPixel;
    private RectTransform Map_Background_Dimensions;

    //private float deltaX, deltaY;

    //private float ValueToClampY;
    private GameObject raycastTest;

    private RectTransform rectr;
    private float VerticalPixel;

    public void Awake() { }
    private void Start() {
        this.Map_Background_Dimensions = this.MapObj.GetComponent<RectTransform>();
        this.Map_Background_Dimensions.anchoredPosition = new Vector2(0, 0);
        this.Half_Map_Background_Dimensions =
            new Vector2(this.Map_Background_Dimensions.rect.width / 2, this.Map_Background_Dimensions.rect.height / 2);
        this.Half_Viewport_Dimensions = new Vector2(this.Viewport_Dimensions.rect.width / 2, this.Viewport_Dimensions.rect.height / 2);
        this.VerticalPixel = this.Half_Map_Background_Dimensions.y - this.Half_Viewport_Dimensions.y;
        this.HorizontalPixel = this.Half_Map_Background_Dimensions.x - this.Half_Viewport_Dimensions.x;
        Debug.Log(this.VerticalPixel + "  " + this.HorizontalPixel);
    }

    // Update is called once per frame
    private void Update() =>
        this.Map_Background_Dimensions.anchoredPosition = new Vector2(
            Mathf.Clamp(this.Map_Background_Dimensions.anchoredPosition.x, -this.HorizontalPixel, this.HorizontalPixel),
            Mathf.Clamp(this.Map_Background_Dimensions.anchoredPosition.y, -this.VerticalPixel, this.VerticalPixel));
    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnDrag(PointerEventData eventData) {
        this.raycastTest = eventData.pointerCurrentRaycast.gameObject;
        try {
            if (this.raycastTest.name.Contains("Map")) {
                this.rectr = this.raycastTest.GetComponent<RectTransform>();
                this.rectr.anchoredPosition += eventData.delta;
            }
        }

        catch (NullReferenceException ex) {
            Debug.Log("out of bounds");
        }
    }

    public void OnEndDrag(PointerEventData eventData) { }

    // Start is called before the first frame update

    public void OnPointerDown(PointerEventData eventData) { }
}
