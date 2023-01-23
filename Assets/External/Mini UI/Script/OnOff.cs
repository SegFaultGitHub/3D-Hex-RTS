using UnityEngine;

public class OnOff : MonoBehaviour {
    [SerializeField] private GameObject textOFF;
    [SerializeField] private GameObject textON;
    [SerializeField] private Switch_Slide switchSlide;

    private void Start() => this.textON.SetActive(false);

    private void Update() {
        if (this.switchSlide.textIsOn) {
            this.SetOn();
        } else {
            this.SetOff();
        }
    }
    public void SetOn() {
        this.textON.SetActive(true);
        this.textOFF.SetActive(false);
    }

    public void SetOff() {
        this.textON.SetActive(false);
        this.textOFF.SetActive(true);
    }
}
