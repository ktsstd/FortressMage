using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonsterAI
{
    private float[] BossMonsterSkillCooldowns = { 30f, 30f, 30f, 30f, 30f, 30f };
    public float[] BossMonsterSkillTimers = new float[6];  // �� ��ų�� ���� ��Ÿ���� �����ϴ� �迭

    private float AllSkillCooldown = 5f;  // ��ü ��ų ��Ÿ��
    public float AllSkillCooldownTimer;  // ��ü ��ų ��Ÿ�� Ÿ�̸�

    private bool isBossPatern = false;  // ���� ���� Ȱ��ȭ ����

    public override void Start()
    {
        base.Start();  // �θ� Ŭ������ Start() ȣ��
        MaxHp = 200f;  // ü�� �ʱ�ȭ
        MonsterDmg = 10;  // ���� ������ �ʱ�ȭ
        CurHp = MaxHp;  // ü�� ����
        isBossPatern = false;
        GameObject playerObject = GameObject.FindWithTag("Player");  // "Player" �±׸� ���� ������Ʈ ã��
        if (playerObject != null)
        {
            player = playerObject.transform;  // �÷��̾��� ��ġ ����
        }
    }

    public override void Update()
    {
        base.Update();
        //float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        //if (distanceToPlayer > attackRange)
        //{
        //    if (!isBossPatern)
        //    {
        //        agent.SetDestination(player.position);
        //    }

        //    else
        //    {
        //        return;
        //    }
        //}
        //else
        //{
        //    StartCoroutine(BossPaternStart());
        //}
        StartCoroutine(BossPaternStart());

        // �� ��ų Ÿ�̸� ����
        for (int i = 0; i < BossMonsterSkillTimers.Length; i++)
        {
            if (BossMonsterSkillTimers[i] > 0f)
            {
                BossMonsterSkillTimers[i] -= Time.deltaTime;
            }
        }

        if (AllSkillCooldownTimer > 0f)
        {
            AllSkillCooldownTimer -= Time.deltaTime;
        }
    }

    private int GetRandomSkill()
    {
        List<int> availableSkills = new List<int>();

        // ��� ��ų�� ���� ��Ÿ���� �������� ����� �� �ִ� ��ų ��Ͽ� �߰�
        for (int i = 0; i < BossMonsterSkillTimers.Length; i++)
        {
            if (BossMonsterSkillTimers[i] <= 0f)  // ��Ÿ���� ���� ��ų��
            {
                availableSkills.Add(i);
            }
        }

        // ��� ������ ��ų�� �ִٸ� �������� ����
        if (availableSkills.Count > 0)
        {
            return availableSkills[Random.Range(0, availableSkills.Count)];
        }
        return -1;  // ����� �� �ִ� ��ų�� ���ٸ� -1 ��ȯ
    }

    private void UseSkill(int skillIndex)
    {
        switch (skillIndex)
        {
            case 0:
                StartCoroutine(BossSkill1());
                break;
            case 1:
                StartCoroutine(BossSkill2());
                break;
            case 2:
                StartCoroutine(BossSkill3());
                break;
            case 3:
                StartCoroutine(BossSkill4());
                break;
            case 4:
                StartCoroutine(BossSkill5());
                break;
            case 5:
                StartCoroutine(BossSkill6());
                break;
            default:
                Debug.LogWarning("Invalid skill index.");
                break;
        }
    }

    private IEnumerator BossSkill1()
    {
        Debug.Log("���� �հ��� ƨ��� ���");
        // �����ٸ� ��ȯ�ϴ� �ڵ� ����
        isBossPatern = false;
        yield break;
    }

    private IEnumerator BossSkill2()
    {
        Debug.Log("���� �ذ�����ü �߻��ϴ� �ִϸ��̼�");
        Debug.Log("���� �ذ�߻�");
        Debug.Log("�÷��̾� ���� �θ���");
        isBossPatern = false;
        yield break;
    }

    private IEnumerator BossSkill3()
    {
        Debug.Log("���� ������ ������ �ִϸ��̼�");
        Debug.Log("���� ��ο� ���� ���������� �ʵ� ���� ����Ʈ");
        Debug.Log("���� ī�޶� ��ο���"); // �ٷ� ��ο��� (ī���̾ƴ϶� ���ϰ�������)
        isBossPatern = false;
        yield break;
    }

    private IEnumerator BossSkill4()
    {
        Debug.Log("���� �հ������� �ϴ� ����Ű�� �ִϸ��̼�");
        Debug.Log("���� �����â �������� ����Ʈ");
        // �����â �ڵ� ����
        isBossPatern = false;
        yield break;
    }

    private IEnumerator BossSkill5()
    {
        Debug.Log("���� �չٴ� �΋H���� �ִϸ��̼�");
        // �÷��̾� �Ѱ����� ������ �ڵ� ����
        Debug.Log("�÷��̾� �ӹ� �θ���");
        // �� �ڸ��� ��ұ�ü ����Ʈ���� �ڵ� ����
        isBossPatern = false;
        yield break;
    }

    private IEnumerator BossSkill6()
    {
        Debug.Log("���� �Ͼ�� ���");
        // �׾��� ���� �ǻ츮�� �ڵ� << ��ǥ���� �߰��� �����
        isBossPatern = false;
        yield break;
    }

    private IEnumerator BossPaternStart()
    {
        if (isBossPatern) yield break;
        isBossPatern = true;

        if (AllSkillCooldownTimer <= 0f)
        {
            int selectedSkill = GetRandomSkill();

            if (selectedSkill != -1)
            {
                BossMonsterSkillTimers[selectedSkill] = BossMonsterSkillCooldowns[selectedSkill];
                AllSkillCooldownTimer = AllSkillCooldown;

                UseSkill(selectedSkill);
            }
        }

        yield break;
    }
}
