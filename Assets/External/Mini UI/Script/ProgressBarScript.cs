using UnityEngine;
using UnityEngine.UI;

public class ProgressBarScript : MonoBehaviour {
    [Space]
    public float targetValue;

    [Space]
    public Image fillImage;
    public Image backGroundImage;
    public InputField inputField;
    public Button addBtn;
    private float backGroundSize;
    private float calcValue;
    private float currentPositionX;
    private float currentValue;
    private RectTransform fillImageRecTransform;

    private float incrementValue;
    private string input;
    private float totalvalue;


    private void Start() {
        this.backGroundSize = this.backGroundImage.rectTransform.rect.width;

        this.addBtn.onClick.AddListener(this.Add);

        this.currentPositionX = this.fillImage.rectTransform.anchoredPosition.x;

        this.incrementValue = this.backGroundSize / this.targetValue;

    }


    public void Add() {
        this.input = this.inputField.text;

        float inVal = float.Parse(this.input);

        this.AddProggress(inVal);
    }

    public void AddProggress(float value) {
        if (value > this.targetValue) {
            this.totalvalue = this.targetValue;

            this.currentValue = this.totalvalue * this.incrementValue;

            this.calcValue = this.currentPositionX + this.currentValue;

            if (this.fillImage.rectTransform.anchoredPosition.x != 0 && this.totalvalue <= this.targetValue) {
                this.fillImage.rectTransform.anchoredPosition = new Vector2(this.calcValue, 0);
            }
        } else if (value < this.targetValue) {
            this.totalvalue += value;

            this.currentValue = this.totalvalue * this.incrementValue;

            this.calcValue = this.currentPositionX + this.currentValue;

            if (this.fillImage.rectTransform.anchoredPosition.x != 0 && this.totalvalue <= this.targetValue) {
                this.fillImage.rectTransform.anchoredPosition = new Vector2(this.calcValue, 0);
            }
        }



    }
}
