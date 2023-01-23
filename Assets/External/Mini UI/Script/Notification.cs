using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour {
    public Text outPutText;

    public void OutPutNotification(string text) => this.outPutText.text = text + "  Is Now Your Friend";
}
