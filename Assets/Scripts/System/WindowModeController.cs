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
        // 드롭다운 옵션 설정
        List<string> options = new List<string> {
            "전체화면",
            "창모드"
        };

        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.onValueChanged.AddListener(index => ChangeFullScreenMode((ScreenMode)index));

        // PlayerPrefs에서 저장된 화면 모드 값 불러오기
        int savedScreenMode = PlayerPrefs.GetInt("ScreenMode", 0); // 기본값은 0 (전체화면)
        dropdown.value = savedScreenMode;

        // 초기 설정
        SetScreenMode((ScreenMode)savedScreenMode);
    }

    /// <summary>
    /// 화면 모드를 설정합니다.
    /// </summary>
    /// <param name="mode">변경할 화면 모드</param>
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

        // 화면 모드를 PlayerPrefs에 저장
        PlayerPrefs.SetInt("ScreenMode", (int)mode);
        PlayerPrefs.Save();  // 변경 사항을 저장
    }

    /// <summary>
    /// 화면 모드를 변경합니다.
    /// </summary>
    /// <param name="mode">변경할 화면 모드</param>
    private void ChangeFullScreenMode(ScreenMode mode)
    {
        SetScreenMode(mode);  // 화면 모드 변경
    }
}