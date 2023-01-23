using UnityEngine;

public class ItemDescriptionCloseButton : MonoBehaviour {
    public ItemDescription itemDescription;

    public void Close() {
        foreach (GameObject gameObject in this.itemDescription.objectList) {
            gameObject.SetActive(false);
        }
        this.itemDescription.windowIsActive = false;
        this.itemDescription.itemDescriptionWindow.SetActive(false);
    }
}
