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
        // �����Ϳ��� ���� ���� ���� ������ �������� �ʰ�, ����Ƽ �����͸� �����ϴ� ���
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // ���� ������ ���� ���� ��� ���ø����̼� ����
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
