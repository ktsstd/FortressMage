using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class ToggleObject : MonoBehaviour
{
    public GameObject objectToToggle;


    public void ExitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            objectToToggle.SetActive(!objectToToggle.activeSelf);
        }
    }


    void OnQuitButtonClicked()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;

    }
}
