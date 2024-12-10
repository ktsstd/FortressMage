using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldManager : MonoBehaviour
{
    public int goldAmount = 0; // 현재 보유하고 있는 골드
    public TMP_Text[] goldText;

    private void Start()
    {
        UpdateGoldUI(); // 초기 UI 업데이트
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
            return true; // 성공적으로 골드를 사용
        }
        return false; // 골드가 부족함
    }

    private void UpdateGoldUI()
    {
        foreach(TMP_Text text in goldText)
            {
            text.text = "Gold: " + goldAmount; // 각 텍스트 요소에 골드 금액을 업데이트
        }
    }
}
