using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleObject : MonoBehaviour
{
    public Slider sizeSlider;
    public Text valueText;
    public RectTransform uiSize1;
    public RectTransform uiSize2;

    public GameObject objectToToggle;

    void Start()
    {
        valueText.text = sizeSlider.value.ToString();

        sizeSlider.onValueChanged.AddListener(OnSliderValueChanged);
        
        UpdateUIElements();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            objectToToggle.SetActive(!objectToToggle.activeSelf);
        }
    }

    void OnSliderValueChanged(float value)
    {
        valueText.text = value.ToString(); 
        UpdateUIElements();
    }

    void UpdateUIElements()
    {
        float newSize = sizeSlider.value;

        uiSize1.sizeDelta = new Vector2(newSize, newSize); 
        uiSize2.sizeDelta = new Vector2(newSize, newSize);
    }
}
