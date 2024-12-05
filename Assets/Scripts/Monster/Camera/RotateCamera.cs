using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    private float rotationSpeed = 10f;  // 회전 속도
    private float currentRotation = 0f;  // 현재 회전 각도
    private bool rotatingTo90 = true;   // 90도로 회전할지, -90도로 회전할지 결정하는 변수

    void Update()
    {
        // 카메라 회전 함수 호출
        HandleRotation();
    }

    void HandleRotation()
    {
        // 회전 방향에 따른 회전
        if (rotatingTo90)
        {
            currentRotation += rotationSpeed * Time.deltaTime;  // 90도로 회전
            if (currentRotation >= 90f)
            {
                currentRotation = 90f;  // 90도에 도달한 후에는 더 이상 회전하지 않음
                rotatingTo90 = false;   // 이제 -90도로 회전 시작
            }
        }
        else
        {
            currentRotation -= rotationSpeed * Time.deltaTime;  // -90도로 회전
            if (currentRotation <= -90f)
            {
                currentRotation = -90f;  // -90도에 도달한 후에는 더 이상 회전하지 않음
                rotatingTo90 = true;     // 이제 90도로 회전 시작
            }
        }

        // 카메라의 Y축 회전 적용
        transform.rotation = Quaternion.Euler(0f, currentRotation, 0f);
    }
}
