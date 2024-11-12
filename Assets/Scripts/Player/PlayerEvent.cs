using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvent : MonoBehaviour
{
    private PlayerController controller;
    void Start()
    {
        controller = GetComponent<PlayerController>(); // 각 플레이어들의 스크립트 가져오기
    }

    // 여기서 이벤트 PlayerContorller 스크립트의 메소드 실행 ex) 레벨업, cc기 등등
}
