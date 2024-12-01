using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Systemkey : MonoBehaviour
{
    // 활성화/비활성화 할 오브젝트를 참조할 변수
    public GameObject objectToToggle;

    // Update 메서드에서 ESC 키 입력을 체크
    void Update()
    {
        // ESC 키가 눌렸을 때
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 오브젝트의 활성화 상태를 토글 (켜거나 끄기)
            objectToToggle.SetActive(!objectToToggle.activeSelf);
        }
    }
}
