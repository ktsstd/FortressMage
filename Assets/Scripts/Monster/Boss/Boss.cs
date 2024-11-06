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
        base.Start(); // �θ� Ŭ������ Start() ȣ��
        MaxHp = 200f; // ü���ʱ�ȭ
        MonsterDmg = 10; // ���� ������ �ʱ�ȭ
        CurHp = MaxHp; // ü��Ȯ��
        isBossPatern = false;
        GameObject playerObject = GameObject.FindWithTag("Player"); // "Player" �±׸� ���� ������Ʈ ã��
        if (playerObject != null)
        {
            player = playerObject.transform; // �÷��̾� ��ȯ ��ü ��������
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
                // �÷��̾ ���� �̵� (��ֹ��� �ڵ����� ��ȸ)
                agent.SetDestination(player.position);
            }

            else
            {
                return;
            }
        }
        else
        {
            // �÷��̾
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
