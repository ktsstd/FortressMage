using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUi : MonoBehaviour
{
    public Image icon;
    public Image[] skills;
    public Image[] skillsback;

    public Image[] elementalSet;

    public Slider playerHpSlider;
    public Slider mixSkillSlider;

    public TextMeshProUGUI playerLvText;

    public float playerLv;
    public float playerMaxHp;
    public float playerHp;
    public float[] skillsCoolTime;
    public float[] skillsMaxCoolTime;

    void Update()
    {
        playerHpSlider.value = playerHp / playerMaxHp;
        for (int i = 0; i < 3; i++)
        {
            skills[i].fillAmount = 1 - (skillsCoolTime[i] / skillsMaxCoolTime[i]);
        }
        playerLvText.text = "LV. " + playerLv;
    }

    public void StartUISetting(Sprite _icon, Sprite[] _skills)
    {
        icon.sprite = _icon;
        for (int i = 0; i <3; i++)
        {
            skills[i].sprite = _skills[i];
            skillsback[i].sprite = _skills[i];
        }
    }
}
