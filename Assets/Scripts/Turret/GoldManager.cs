using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldManager : MonoBehaviour
{
    public int goldAmount = 0; // ���� �����ϰ� �ִ� ���
    public TextMeshProUGUI goldText;

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
        if (goldText != null)
        {
            goldText.text = "Gold: " + goldAmount; // UI �ؽ�Ʈ ������Ʈ
        }
    }
}
