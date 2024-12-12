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
        // 에디터에서 실행 중일 때는 게임을 종료하지 않고, 유니티 에디터를 종료하는 기능
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // 실제 게임이 실행 중인 경우 애플리케이션 종료
            Application.Quit();
#endif
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

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
