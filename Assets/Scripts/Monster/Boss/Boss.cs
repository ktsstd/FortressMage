using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonsterAI
{
    private float BossMonsterSkillCooldown1 = 30f;
    private float BossMonsterSkillCooldown2 = 30f;
    private float BossMonsterSkillCooldown3 = 30f;
    private float BossMonsterSkillCooldown4 = 30f;
    private float BossMonsterSkillCooldown5 = 30f;
    private float BossMonsterSkillCooldown6 = 30f;

    private float BossMonsterSkillTimer1;
    private float BossMonsterSkillTimer2;
    private float BossMonsterSkillTimer3;
    private float BossMonsterSkillTimer4;
    private float BossMonsterSkillTimer5;
    private float BossMonsterSkillTimer6;

    private bool isBossPatern = false;

    public override void Start()
    {
        base.Start(); // 부모 클래스의 Start() 호출
        MaxHp = 200f; // 체력초기화
        MonsterDmg = 10; // 몬스터 데미지 초기화
        CurHp = MaxHp; // 체력확인
        isBossPatern = false;
        GameObject playerObject = GameObject.FindWithTag("Player"); // "Player" 태그를 가진 오브젝트 찾기
        if (playerObject != null)
        {
            player = playerObject.transform; // 플레이어 변환 객체 가져오기
        }
    }

    public override void Update()
    {
        base.Update();
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > attackRange)
        {
            if (!isBossPatern)
            {
                // 플레이어를 향해 이동 (장애물은 자동으로 우회)
                agent.SetDestination(player.position);
            }

            else
            {
                return;
            }
        }
        else
        {
            // 플레이어가
            StartCoroutine(BossPaternStart());
        }
    }

    private IEnumerator BossPaternStart()
    {
        isBossPatern = true;
        //switch:
        yield break;
            
    }
}
