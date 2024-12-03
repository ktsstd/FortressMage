using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonsterAI
{
    private Transform closestTarget;
    private Animator animator;
    // private ParticleSystem

    private float[] BossMonsterSkillCooldowns = { 30f, 30f, 30f, 30f };
    public float[] BossMonsterSkillTimers = new float[4];  // �� ��ų�� ���� ��Ÿ���� �����ϴ� �迭

    private float AllSkillCooldown = 5f;  // ��ü ��ų ��Ÿ��
    public float AllSkillCooldownTimer;  // ��ü ��ų ��Ÿ�� Ÿ�̸�

    private float S1Speed = 0f;

    private bool isBossPatern = false;  // ���� ���� Ȱ��ȭ ����

    public GameObject BossSkill1Obj; 

    public override void Start()
    {
        base.Start();  // �θ� Ŭ������ Start() ȣ��
        MaxHp = 200f;  // ü�� �ʱ�ȭ
        MonsterDmg = 10;  // ���� ������ �ʱ�ȭ
        attackRange = 78.0f; 
        CurHp = MaxHp;  // ü�� ����
        isBossPatern = false;
        animator = GetComponent<Animator>();
        StartCoroutine(StartRotate());
    }

    private IEnumerator StartRotate()
    {
        Transform BossSkilObjT = BossSkill1Obj.transform;
        while(S1Speed == 300f)
        {
            S1Speed *= Time.deltaTime;
            BossSkilObjT.transform.Rotate(0, S1Speed, 0);
        }
        yield break;
    }

    // public override void Update()
    // {
    //     StartCoroutine(BossPaternStart());

    //     // �� ��ų Ÿ�̸� ����
    //     for (int i = 0; i < BossMonsterSkillTimers.Length; i++)
    //     {
    //         if (BossMonsterSkillTimers[i] > 0f)
    //         {
    //             BossMonsterSkillTimers[i] -= Time.deltaTime;
    //         }
    //     }

    //     if (AllSkillCooldownTimer > 0f)
    //     {
    //         AllSkillCooldownTimer -= Time.deltaTime;
    //     }
    // }

    public override void Update()
    {
        if (!isBossPatern && !NoTarget)
        {
            closestTarget = GetClosestTarget();
        }
        
        if (closestTarget != null)
        {
            float sqrDistanceToTarget = (closestTarget.position - transform.position).sqrMagnitude;

            if (sqrDistanceToTarget > attackRange * attackRange)
            {
                if (!isBossPatern)
                {
                    agent.SetDestination(closestTarget.position);
                }
            }
            else
            {
                agent.ResetPath();

                if (attackTimer <= 0f && !isBossPatern)
                {
                    isBossPatern = true;
                    StartCoroutine(BossPaternStart());
                }
            }
        }
        else
        {
            // closestTarget = GetClosestTarget();
            NoTarget = true;
            GameObject castleObj = GameObject.FindWithTag("Castle");
            closestTarget = castleObj.transform;
        }

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

    private Transform GetClosestTarget()
    {
        float closestSqrDistance = Mathf.Infinity;
        Transform closestTarget = null;

        string[] tags = { "skilltower", "turret", "Player" };

        foreach (string tag in tags)
        {
            GameObject[] targetsWithTag = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject targetObj in targetsWithTag)
            {
                Transform target = targetObj.transform;
                if (target == null) continue;

                float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;

                // 조건을 확인하여 가장 가까운 대상을 찾음
                if (tag == "turret")
                {
                    Turret towerScript = target.GetComponent<Turret>();
                    if (towerScript != null)
                    {
                        if (!towerScript.canAttack) continue;
                        else
                        {
                            if (sqrDistanceToTarget < closestSqrDistance)
                            {
                                closestSqrDistance = sqrDistanceToTarget;
                                closestTarget = target;
                            }
                        }
                    }
                }

                if (tag == "Player")
                {
                    PlayerController playerScript = target.GetComponent<PlayerController>();
                    if (playerScript != null)
                    {
                        if (playerScript.isDie) continue;
                        else
                        {
                            if (sqrDistanceToTarget < closestSqrDistance)
                            {
                                closestSqrDistance = sqrDistanceToTarget;
                                closestTarget = target;
                            }
                        }
                    }
                }
                if (tag == "skilltower")
                {
                    Skilltower skilltowerScript = target.GetComponent<Skilltower>();
                    if (skilltowerScript != null)
                    {
                        if (!skilltowerScript.canAttack) continue;
                        else
                        {
                            if (sqrDistanceToTarget < closestSqrDistance)
                            {
                                closestSqrDistance = sqrDistanceToTarget;
                                closestTarget = target;
                            }
                        }
                    }
                }
            }
        }

        return closestTarget;
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
            default:
                Debug.LogWarning("Invalid skill index.");
                break;
        }
    }

    private IEnumerator BossSkill1()
    {
        isBossPatern = false;
        yield break;
    }

    private IEnumerator BossSkill2()
    {
        isBossPatern = false;
        yield break;
    }

    private IEnumerator BossSkill3()
    {
        isBossPatern = false;
        yield break;
    }

    private IEnumerator BossSkill4()
    {
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
