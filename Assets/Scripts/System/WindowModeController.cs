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
            "전체화면",
            "창모드"
        };

        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.onValueChanged.AddListener(index => ChangeFullScreenMode((ScreenMode)index));

        // 초기 설정
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
    /// 스크린의 전체 스크린 모드를 변경합니다.
    /// </summary>
    /// <param name="mode">변경할 스크린 모드</param>
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
