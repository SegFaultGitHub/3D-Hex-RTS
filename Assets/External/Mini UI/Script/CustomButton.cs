using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    [SerializeField] private GameObject NormalButton;
    [SerializeField] private GameObject PressedButton;


    public void OnPointerDown(PointerEventData eventData) {
        this.NormalButton.SetActive(false);
        this.PressedButton.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData) {
        this.NormalButton.SetActive(true);
        this.PressedButton.SetActive(false);
        this.Pressed();
    }

    public void Pressed() => Debug.Log(" Called From Press Up");
}
