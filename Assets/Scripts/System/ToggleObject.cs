using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    public GameObject objectToToggle;

    void Update()
    {
        // ESC 키가 눌렸을 때
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 오브젝트의 활성화 상태를 토글
            objectToToggle.SetActive(!objectToToggle.activeSelf);
        }
    }
}
