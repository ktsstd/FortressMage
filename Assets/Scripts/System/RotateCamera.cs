using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    private float rotationSpeed = 2;  // ȸ�� �ӵ�
    private float currentRotation = 0f;  // ���� ȸ�� ����
    private bool rotatingTo90 = true;   // 90���� ȸ������, -90���� ȸ������ �����ϴ� ����

    void Update()
    {
        // ī�޶� ȸ�� �Լ� ȣ��
        HandleRotation();
    }

    void HandleRotation()
    {
        // ȸ�� ���⿡ ���� ȸ��
        if (rotatingTo90)
        {
            currentRotation += rotationSpeed * Time.deltaTime;  // 90���� ȸ��
            if (currentRotation >= 50f)
            {
                currentRotation = 50f;  // 90���� ������ �Ŀ��� �� �̻� ȸ������ ����
                rotatingTo90 = false;   // ���� -90���� ȸ�� ����
            }
        }
        else
        {
            currentRotation -= rotationSpeed * Time.deltaTime;  // -90���� ȸ��
            if (currentRotation <= -50f)
            {
                currentRotation = -50f;  // -90���� ������ �Ŀ��� �� �̻� ȸ������ ����
                rotatingTo90 = true;     // ���� 90���� ȸ�� ����
            }
        }

        // ī�޶��� Y�� ȸ�� ����
        transform.rotation = Quaternion.Euler(40f, currentRotation, 0f);
    }
}
