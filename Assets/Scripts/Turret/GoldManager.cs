using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldManager : MonoBehaviour
{
    public int goldAmount = 0; // ���� �����ϰ� �ִ� ���
    public TMP_Text[] goldText;

    private void Start()
    {
        UpdateGoldUI(); // �ʱ� UI ������Ʈ
        AddGold(10000);
    }

    public void AddGold(int amount)
    {
        goldAmount += amount;
        UpdateGoldUI();
    }

    public bool SpendGold(int amount)
    {
        if (goldAmount >= amount)
        {
            goldAmount -= amount;
            UpdateGoldUI(); 
            return true; // ���������� ��带 ���
        }
        return false; // ��尡 ������
    }

    private void UpdateGoldUI()
    {
        foreach(TMP_Text text in goldText)
            {
            text.text = "Gold: " + goldAmount; // �� �ؽ�Ʈ ��ҿ� ��� �ݾ��� ������Ʈ
        }
    }
}
