using UnityEngine;
using UnityEngine.UI;

public class ViewSelector : MonoBehaviour {
    public Button AchievementViewButton;
    public Button OtherViewButton;

    public GameObject AchievementViewPanel;
    public GameObject OtherViewPanel;

    public void Start() {
        this.AchievementViewButton.onClick.AddListener(this.Achievement);
        this.OtherViewButton.onClick.AddListener(this.OtherView);

        this.AchievementViewPanel.SetActive(true);
        this.OtherViewPanel.SetActive(false);
    }
    public void Achievement() {
        this.AchievementViewPanel.SetActive(true);
        this.OtherViewPanel.SetActive(false);
    }

    public void OtherView() {
        this.OtherViewPanel.SetActive(true);
        this.AchievementViewPanel.SetActive(false);
    }
}
