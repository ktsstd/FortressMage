using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Exp : MonoBehaviour
{
    private float maxExp = 100; 
    private float curExp = 0; 

    public int Lv = 1;
    public TextMeshProUGUI LvText;
    public Image Xp_Bar;

    void Start()
    {
        Player_XP();
    }

    void Update()
    {
        LvText.text = "Lv" + Lv;
        Xp_Bar.fillAmount = curExp / maxExp;
        if (Input.GetKeyDown(KeyCode.E))
        {
            curExp += 10; 
        }
        Lv_UP();
    }

    public void Player_XP()
    {
        maxExp = Lv * 100;
    }

    public void Lv_UP()
    {
        if (curExp >= maxExp)
        {
            curExp -= maxExp;
            Lv++;
            Player_XP();
        }
    }
}