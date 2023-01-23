using UnityEngine;
using UnityEngine.UI;

public class StartScript : MonoBehaviour {
    public GameObject loading;
    public LoadingScript loadingScript;
    public Button play;
    public GameObject startingPage;
    public GameObject gemsAndCoin;
    public GameObject DemoSelect;
    public Menus menuScript;


    private void Start() => this.play.onClick.AddListener(this.StartButton);
    public void StartButton() {
        this.loading.SetActive(true);
        this.StartCoroutine(this.loadingScript.loading());
        this.startingPage.SetActive(false);

    }
}
