using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstEliteMonster : MonsterAI
{
    private int MonsterShield = 60; // 몬스터의 쉴드 값
    private int CurShield = 0;

    // 스킬 쿨타임 설정
    // private float skillCooldown = 5f;
    private float skillTimer = 0f; // 스킬 타이머

    private float MaxHp40Per;

    private bool isShielded = false;
    private bool isShield = false;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();    
        MaxHp = 200f;
        CurHp = MaxHp;
        MaxHp40Per = MaxHp * 0.4f;
        MonsterDmg = 10; // 몬스터 데미지 초기화
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
