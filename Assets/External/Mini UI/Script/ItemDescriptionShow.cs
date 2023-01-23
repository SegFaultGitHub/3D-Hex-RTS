using UnityEngine;

public class ItemDescriptionShow : MonoBehaviour {
    public ItemDescription itemDescription;
    public void PressButton() {
        if (this.itemDescription.windowIsActive == false) {
            string itemName = this.name;
            this.itemDescription.ShowItemDescription(itemName);
        }

        //Debug.Log(itemName);



    }
}
