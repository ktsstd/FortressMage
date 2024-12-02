using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImg : MonoBehaviour

{
    // ����� �̹����� ǥ���� Image ������Ʈ
    public Image targetImage;

    // ��ư Ŭ�� �� ����� �̹�����
    public Sprite[] newSprites;

    // ��ư Ŭ�� �� �̹����� �����ϴ� �Լ�
    public void OnButtonClick(int index)
    {
        // index ���� �ش��ϴ� �̹����� ����
        if (targetImage != null && newSprites != null && newSprites.Length > index)
        {
            targetImage.sprite = newSprites[index];
        }
    }
}

