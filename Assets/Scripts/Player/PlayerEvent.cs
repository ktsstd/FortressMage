using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvent : MonoBehaviour
{
    private PlayerController controller;
    void Start()
    {
        controller = GetComponent<PlayerController>(); // �� �÷��̾���� ��ũ��Ʈ ��������
    }

    // ���⼭ �̺�Ʈ PlayerContorller ��ũ��Ʈ�� �޼ҵ� ���� ex) ������, cc�� ���
}
