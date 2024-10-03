using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    private float maxHp = 100; 
    private float curHp = 100;

    public Image HpBar;

    void Start()
    {
        
    }

    void Update()
    {
        HpBar.fillAmount = curHp / maxHp;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            curHp -= 10;
        }
    }
}
