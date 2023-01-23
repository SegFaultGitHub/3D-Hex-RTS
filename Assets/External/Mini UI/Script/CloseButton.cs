using UnityEngine;

public class CloseButton : MonoBehaviour {
    public GameObject Panel;
    private void Start() { }

    public void Close() {
        this.Panel.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
