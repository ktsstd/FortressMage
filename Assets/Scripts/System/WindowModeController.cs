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
        List<string> options = new List<string> {
            "��üȭ��",
            "â���"
        };

        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.onValueChanged.AddListener(index => ChangeFullScreenMode((ScreenMode)index));

        // �ʱ� ����
        switch (dropdown.value)
        {
            case 0:
                Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
                break;
            case 1:
                Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
                break;
        }
    }

    /// <summary>
    /// ��ũ���� ��ü ��ũ�� ��带 �����մϴ�.
    /// </summary>
    /// <param name="mode">������ ��ũ�� ���</param>
    private void ChangeFullScreenMode(ScreenMode mode)
    {
        switch (mode)
        {
            case ScreenMode.FullScreenWindow:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case ScreenMode.Window:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
    }
}
