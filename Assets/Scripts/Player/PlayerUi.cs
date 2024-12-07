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

    public Image mixSkill;

    public Image[] elementalSet;

    public Slider playerHpSlider;
    public Slider mixSkillSlider;

    public TextMeshProUGUI playerLvText;

    public float playerLv;
    public float playerMaxHp;
    public float playerHp;
    public float mixCooldown;
    public float mixMaxCooldown;
    public float[] skillsCoolTime;
    public float[] skillsMaxCoolTime;

    public float[] elementalSetCoolTime;
    public float[] elementalSetMaxCoolTime;

    void Update()
    {
        playerHpSlider.value = playerHp / playerMaxHp;
        for (int i = 0; i < 3; i++)
        {
            skills[i].fillAmount = 1 - (skillsCoolTime[i] / skillsMaxCoolTime[i]);
        }
        for (int i = 0; i < 2; i++)
        {
            elementalSet[i].fillAmount = 1 - (elementalSetCoolTime[i] / elementalSetMaxCoolTime[i]);
        }
        playerLvText.text = "LV. " + playerLv;

        mixSkillSlider.value = 1 - (mixCooldown / mixMaxCooldown);
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
