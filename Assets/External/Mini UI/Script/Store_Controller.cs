using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Store_Controller : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public GameObject Coin_view;
    public GameObject Gem_view;

    public GameObject Gems_On_Image;
    public GameObject Gems_Pressed_Image;
    public GameObject Gems_Off_Image;
    public GameObject Coins_On_Image;
    public GameObject Coins_Pressed_Image;
    public GameObject Coins_Off_Image;

    public Button gemButton;
    public Button coinButton;
    // Start is called before the first frame update
    private void Start() {

        this.Coin_view.SetActive(false);
        this.Gem_view.SetActive(true);
        this.Gems_Off_Image.SetActive(false);
        this.Coins_On_Image.SetActive(false);
        this.Gems_On_Image.SetActive(false);
        this.Coins_Off_Image.SetActive(true);
        this.Coins_Pressed_Image.SetActive(false);
        this.Gems_Pressed_Image.SetActive(true);
    }

    // Update is called once per frame
    private void Update() { }

    public void OnPointerDown(PointerEventData eventData) => Debug.Log("from pointer down");

    public void OnPointerUp(PointerEventData eventData) { }

    public void GemButton() {
        this.Coins_Off_Image.SetActive(true);
        this.Coins_On_Image.SetActive(false);
        this.Coins_Pressed_Image.SetActive(false);
        //Gems_On_Image.SetActive(true);
        this.Gems_Pressed_Image.SetActive(true);
        this.Gems_Off_Image.SetActive(false);

        this.Gem_view.SetActive(true);
        this.Coin_view.SetActive(false);
    }

    public void CoinButton() {
        this.Gems_On_Image.SetActive(false);
        this.Gems_Off_Image.SetActive(true);
        this.Gems_Pressed_Image.SetActive(false);

        this.Coins_Pressed_Image.SetActive(true);
        //Coins_On_Image.SetActive(true);
        this.Coins_Off_Image.SetActive(false);

        this.Coin_view.SetActive(true);
        this.Gem_view.SetActive(false);
    }
}
