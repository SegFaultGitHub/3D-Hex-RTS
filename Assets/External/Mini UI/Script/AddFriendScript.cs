using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AddFriendScript : MonoBehaviour {
    [SerializeField] private GameObject notification;
    [SerializeField] private Notification notificationScript;
    [SerializeField] private Button addButton;
    [SerializeField] private Text nameText;

    public SlideAnimation slideAnimation;

    private IEnumerator coroutine;

    private string Name;
    private float startPos;
    private void Start() {
        this.coroutine = this.StartSlideoutCoroutine();
        this.notification.SetActive(false);
        //addButton.onClick.AddListener(addFriend);
        this.Name = this.nameText.text;
        this.startPos = this.notification.GetComponent<RectTransform>().anchoredPosition.x;
    }

    private void addFriend() {
        this.notification.SetActive(true);
        this.notificationScript.OutPutNotification(this.Name);
        this.slideAnimation.anim.Play("SlideIn");
        this.Invoke("StopSlide", 3);
    }

    private IEnumerator StartSlideoutCoroutine() {
        yield return new WaitForSeconds(3);
        this.slideAnimation.anim.Play("SlideOut");
        this.StopSlide();
    }

    private void StopSlide() => this.slideAnimation.anim.Play("SlideOut");
}
