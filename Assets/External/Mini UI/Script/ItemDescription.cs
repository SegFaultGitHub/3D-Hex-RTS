using System.Collections.Generic;
using UnityEngine;

public class ItemDescription : MonoBehaviour {
    public List<GameObject> objectList;
    public GameObject itemDescriptionWindow;
    public bool windowIsActive;

    private void Awake() => this.windowIsActive = false;

    public void ShowItemDescription(string itemSelected) {
        foreach (GameObject gameObject in this.objectList) {
            if (itemSelected == gameObject.name) {
                Debug.Log(gameObject.name + " ");
                if (this.windowIsActive == false) {
                    this.itemDescriptionWindow.SetActive(true);
                    gameObject.SetActive(true);
                    this.windowIsActive = true;
                }
            }
        }
    }
}
