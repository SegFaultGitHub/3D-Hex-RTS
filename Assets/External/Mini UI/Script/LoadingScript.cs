using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour {
    public StartScript startScript;
    public Slider loadingProgress;
    public float loadValue;
    public Menus menusScript;
    public Text loadText;


    private void Start() => this.loadValue = 0.0f;

    public IEnumerator loading() {
        while (this.loadingProgress.value < 100f) {
            this.loadValue += 0.5f;

            this.loadingProgress.value = this.loadValue;
            this.loadText.text = this.loadValue.ToString();
            if (this.loadingProgress.value == 100f) {
                this.StopCoroutine(this.loading());
                //startScript.gemsAndCoin.SetActive(true);
                this.startScript.DemoSelect.SetActive(true);
                this.startScript.loading.SetActive(false);
                //menusScript.backButton.SetActive(true);

            }
            yield return null;
        }
    }

    //public void SendTheme(string theme)
    //{
    //    if (theme == "Light")
    //    {
    //       menusScript.MenuLight.SetActive(true);
    //       menusScript.backButton.SetActive(true);
    //    }
    //    else if (theme == "Dark")
    //    {
    //       Debug.Log("from dark");
    //       menusScript.MenuDark.SetActive(true);
    //       menusScript.backButton.SetActive(true);
    //    }
    //}
}
