using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ToggleObject : MonoBehaviour
{
    public Slider sizeSlider;
    public TMP_Text valueText;
    public RectTransform uiSize1;
    public RectTransform uiSize2;

    public GameObject objectToToggle;

    public Button quitButton;

    private Vector3 initialScale;

    void Start()
    {
        initialScale = uiSize1.localScale;

        sizeSlider.minValue = 1;
        sizeSlider.maxValue = 100;

        sizeSlider.value = 100f;

        valueText.text = sizeSlider.value.ToString("F0");
        sizeSlider.onValueChanged.AddListener(OnSliderValueChanged);

        UpdateUIElements();

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }
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
        valueText.text = value.ToString("F0"); 
        UpdateUIElements();
    }

    void UpdateUIElements()
    {
        float newSize = Mathf.Lerp(0.4f, 1f, sizeSlider.value / 100f);

        uiSize1.localScale = new Vector3(newSize, newSize, 1);
        uiSize2.localScale = new Vector3(newSize, newSize, 1);
    }

    void OnQuitButtonClicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
