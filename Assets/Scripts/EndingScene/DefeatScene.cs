using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DefeatScene : MonoBehaviour
{
    public Image storyImage;
    public Sprite[] storyImages;
    public TextMeshProUGUI storyText;

    int num = 0;

    public void ClickEvent()
    {
        num++;
        switch (num)
        {
            case 1:
                storyImage.sprite = storyImages[0];
                storyText.text = "������ ��迡�� ���۵� Ÿ���� ���ɵ��� ������ ���� �ż�����, ������� �η��� ���� �־���.�̵��� ������ ������ ��õ�� �� �ڿ��� ������� ����� ���Ѱ�, ���ư� ������ ������� �����Ϸ��� ������̾���.�� Ÿ���� ���ɵ��� �θ��� �ڴ� ���ſ� �ݱ� ������ �渶���� ���� ��� �������� �߹���� �̾���, �����θ� ����� ���ֶ� Ī�Ͽ���";
                break;
            case 2:
                storyImage.sprite = storyImages[1];
                storyText.text = "Ȳ���� �� ������ ��Ȳ�� �ذ��ϱ� ���� ���� �ְ��� ������鿡�� ������ ��û�ߴ�. \"�츮�� ������ �������� �ڵ�κ��� �츮�� ���� �鼺���� �����ְԳ�!\"Ȳ���� ��Ҹ��� �����Կ� �� �־���, ��������� ������ ����⿡ ó�� �ִٴ� ����� �ܸ��� �� ������.��������� ���ǰ� �ʿ��� �����̾���.";
                break;
            case 3:
                storyImage.sprite = storyImages[2];
                storyText.text = "���� ���� �¾� �Ʒ�, ������ ���� ���� ���ù��� ��������� �𿴴�.�׵��� ���� �ٸ� ������ ���� ���� �������� ȭ��, ����, ����, �׸��� ������ ������ �ٷ����.�׵��� ������ ���� �������� Ÿ���� ���ɵ��� �ٶ󺸸� ���ƴ�.���츮�� ������ ����� ��Ű�� ���� ���⿡ ������ �׵��� ħ���� ���ƾ� �Ѵ�! ��ҿ� �����϶�!��";
                break;
        }
    }

    public void MoveScene()
    {
        SceneManager.LoadScene("New Scene");
    }
}
