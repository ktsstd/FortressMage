using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Systemkey : MonoBehaviour
{
    // Ȱ��ȭ/��Ȱ��ȭ �� ������Ʈ�� ������ ����
    public GameObject objectToToggle;

    // Update �޼��忡�� ESC Ű �Է��� üũ
    void Update()
    {
        // ESC Ű�� ������ ��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ������Ʈ�� Ȱ��ȭ ���¸� ��� (�Ѱų� ����)
            objectToToggle.SetActive(!objectToToggle.activeSelf);
        }
    }
}
