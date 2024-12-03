using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    public GameObject objectToToggle;

    void Update()
    {
        // ESC Ű�� ������ ��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ������Ʈ�� Ȱ��ȭ ���¸� ���
            objectToToggle.SetActive(!objectToToggle.activeSelf);
        }
    }
}
