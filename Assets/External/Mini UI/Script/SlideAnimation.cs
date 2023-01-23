using UnityEngine;

public class SlideAnimation : MonoBehaviour {


    public Animator anim;

    private void Start() => this.anim = this.GetComponent<Animator>();
}
