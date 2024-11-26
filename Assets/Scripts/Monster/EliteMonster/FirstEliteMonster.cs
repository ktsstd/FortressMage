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
    // private float skillTimer = 0f; // 스킬 타이머

    private float MaxHp40Per;

    private bool isShielded = false;

    // public GameObject[] Obstacles;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();    
        MaxHp = 200f;
        CurHp = MaxHp;
        MaxHp40Per = MaxHp * 0.4f;
        MonsterDmg = 10; // 몬스터 데미지 초기화
        // GameObject[] Obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (CurHp <= MaxHp40Per)
        {
            if (!isShielded)
            {
                Debug.Log("Shield on");
                StartCoroutine(EliteMonsterShield(MonsterShield));
                isShielded = true;
            }
        }
    }

    private IEnumerator EliteMonsterShield(int MonsterShield)
    {
        agent.ResetPath();
        yield return new WaitForSeconds(2f); // 2초 대기
        Debug.Log("Shield");
        CurShield = MonsterShield;
    }

    public override void MonsterDmged(int playerdamage)
    {
        if (CurShield >=0)
        {
            CurShield -= playerdamage;
        }

        else
        {
            CurHp -= playerdamage;
        }
    }
}