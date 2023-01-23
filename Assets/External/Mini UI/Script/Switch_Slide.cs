using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Switch_Slide : MonoBehaviour {

    [SerializeField] private Button btn1;
    [SerializeField] private Image Handle_Image_Off, Handle_Image_On;
    [SerializeField] private Image Background_Image_Off, Background_Image_On;
    [SerializeField] private GameObject Background_Pixel_Dimension;
    [SerializeField] private GameObject Toggle_Pixel_Dimension;
    [SerializeField] private float Offset_Pixel;



    public bool textIsOn;

    public bool isoff;
    private float BG_Center;
    private float BG_Size;
    private float BG_StartPosition;

    private Color currentcolor;

    private Color currentcolor2;
    private Vector2 End_Point;
    private Color newColor;
    private Color newColor2;

    private Vector2 Start_Point;

    private float time;
    private float Toggle_Center;

    private float Toggle_Height;
    private float Toggle_Size;

    // Start is called before the first frame update
    private void Start() {



        this.currentcolor = new Color(this.Handle_Image_Off.color.r, this.Handle_Image_Off.color.g, this.Handle_Image_Off.color.b, 1);

        this.newColor = new Color(this.Handle_Image_Off.color.r, this.Handle_Image_Off.color.g, this.Handle_Image_Off.color.b, 0);

        this.currentcolor2 = new Color(this.Handle_Image_On.color.r, this.Handle_Image_On.color.g, this.Handle_Image_On.color.b, 1);

        this.newColor2 = new Color(this.Handle_Image_On.color.r, this.Handle_Image_On.color.g, this.Handle_Image_On.color.b, 0);

        this.Handle_Image_On.color = this.newColor2;

        this.isoff = true; //Default state is off

        this.Toggle_Size = this.Toggle_Pixel_Dimension.GetComponent<RectTransform>().rect.width; //Width of Toggle in pixels

        this.Toggle_Height = this.Toggle_Pixel_Dimension.GetComponent<RectTransform>().anchoredPosition.y;

        Debug.Log("Toggle Height " + this.Toggle_Height);

        this.Toggle_Center = this.Toggle_Size / 2; // Center Point of the Toggle

        this.BG_Size = this.Background_Pixel_Dimension.GetComponent<RectTransform>().rect.width; //Width of Background in pixels

        this.BG_Center = this.BG_Size / 2; // The half size of the Background.

        this.BG_StartPosition =
            this.BG_Center
            - (this.Offset_Pixel + this.Toggle_Center); // The starting position of the handle, default is on off when it is first run.

        this.Toggle_Pixel_Dimension.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(-this.BG_StartPosition, this.Toggle_Height); //Set the handle to the off position.

        this.Start_Point = new Vector2(-this.BG_StartPosition, this.Toggle_Height);

        this.End_Point = new Vector2(this.BG_StartPosition, this.Toggle_Height);

        Debug.Log(this.End_Point + "  " + this.Start_Point);

    }

    // Update is called once per frame
    private void Update() { }


    //This is the function called when we click the button.
    private void Execute1() { }




    //Base on the state of the switch we start a coroutine to handle the movement of the toggle handle
    public void Switching() {

        if (this.isoff) {
            this.textIsOn = true;
            this.time = 0;
            this.StartCoroutine(this.SwitchCoroutineOn());
            this.btn1.interactable = false;
            this.isoff = false;
        } else {
            this.textIsOn = false;
            this.time = 0;
            this.StartCoroutine(this.SwitchCoroutineOff());
            this.btn1.interactable = false;
            this.isoff = true;
        }
    }


    private IEnumerator SwitchCoroutineOn() {
        while (this.time < 1f) {
            this.time += 0.02f;

            this.Toggle_Pixel_Dimension.GetComponent<RectTransform>().anchoredPosition =
                Vector2.Lerp(this.Start_Point, this.End_Point, this.time);

            this.Handle_Image_Off.color = Color.Lerp(this.currentcolor, this.newColor, this.time);

            this.Handle_Image_On.color = Color.Lerp(this.newColor2, this.currentcolor2, this.time);

            if (Mathf.Round(this.Toggle_Pixel_Dimension.GetComponent<RectTransform>().anchoredPosition.x) == this.End_Point.x) {
                this.Execute1();
                Debug.Log("From on");
                this.btn1.interactable = true;
                this.StopCoroutine(this.SwitchCoroutineOn());
            }
            yield return null;
        }
    }

    private IEnumerator SwitchCoroutineOff() {
        while (this.time < 1f) {
            this.time += 0.02f;

            this.Toggle_Pixel_Dimension.GetComponent<RectTransform>().anchoredPosition =
                Vector2.Lerp(this.End_Point, this.Start_Point, this.time);

            this.Handle_Image_Off.color = Color.Lerp(this.newColor, this.currentcolor, this.time);

            this.Handle_Image_On.color = Color.Lerp(this.currentcolor2, this.newColor2, this.time);

            if (Mathf.Round(this.Toggle_Pixel_Dimension.GetComponent<RectTransform>().anchoredPosition.x) == -this.End_Point.x) {
                this.btn1.interactable = true;
                this.StopCoroutine(this.SwitchCoroutineOff());
            }
            yield return null;
        }
    }
}
