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
    public Image[] buffIcon;

    public Slider playerHpSlider;
    public Slider mixSkillSlider;

    public TextMeshProUGUI playerLvText;

    public bool isCooltimeBuff = false;
    public int shield = 0;
    public bool isAktBuff = false;

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

        if (isCooltimeBuff)
            buffIcon[4].gameObject.SetActive(isCooltimeBuff);
        else
            buffIcon[4].gameObject.SetActive(isCooltimeBuff);

        if (isAktBuff)
            buffIcon[5].gameObject.SetActive(isAktBuff);
        else
            buffIcon[5].gameObject.SetActive(isAktBuff);


        switch (shield)
        {
            case 1:
                buffIcon[0].gameObject.SetActive(true);
                break;
            case 2:
                buffIcon[0].gameObject.SetActive(true);
                buffIcon[1].gameObject.SetActive(true);
                break;
            case 3:
                buffIcon[0].gameObject.SetActive(true);
                buffIcon[1].gameObject.SetActive(true);
                buffIcon[2].gameObject.SetActive(true);
                break;
            case 4:
                buffIcon[0].gameObject.SetActive(true);
                buffIcon[1].gameObject.SetActive(true);
                buffIcon[2].gameObject.SetActive(true);
                buffIcon[3].gameObject.SetActive(true);
                break;
            default:
                buffIcon[0].gameObject.SetActive(false);
                buffIcon[1].gameObject.SetActive(false);
                buffIcon[2].gameObject.SetActive(false);
                buffIcon[3].gameObject.SetActive(false);
                break;
        }
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
