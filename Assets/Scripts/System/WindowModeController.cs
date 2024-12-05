using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WindowModeController : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    public enum ScreenMode
    {
        FullScreenWindow,
        Window,
    }

    private void Start()
    {
        // ��Ӵٿ� �ɼ� ����
        List<string> options = new List<string> {
            "��üȭ��",
            "â���"
        };

        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.onValueChanged.AddListener(index => ChangeFullScreenMode((ScreenMode)index));

        // PlayerPrefs���� ����� ȭ�� ��� �� �ҷ�����
        int savedScreenMode = PlayerPrefs.GetInt("ScreenMode", 0); // �⺻���� 0 (��üȭ��)
        dropdown.value = savedScreenMode;

        // �ʱ� ����
        SetScreenMode((ScreenMode)savedScreenMode);
    }

    /// <summary>
    /// ȭ�� ��带 �����մϴ�.
    /// </summary>
    /// <param name="mode">������ ȭ�� ���</param>
    private void SetScreenMode(ScreenMode mode)
    {
        switch (mode)
        {
            case ScreenMode.FullScreenWindow:
                Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
                break;
            case ScreenMode.Window:
                Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
                break;
        }

        // ȭ�� ��带 PlayerPrefs�� ����
        PlayerPrefs.SetInt("ScreenMode", (int)mode);
        PlayerPrefs.Save();  // ���� ������ ����
    }

    /// <summary>
    /// ȭ�� ��带 �����մϴ�.
    /// </summary>
    /// <param name="mode">������ ȭ�� ���</param>
    private void ChangeFullScreenMode(ScreenMode mode)
    {
        SetScreenMode(mode);  // ȭ�� ��� ����
    }
}