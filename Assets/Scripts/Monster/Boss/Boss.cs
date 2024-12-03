using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Boss : MonsterAI
{
    private Transform closestTarget;
    private Transform CastlePos;
    private Animator animator;
    // private ParticleSystem

    private float[] BossMonsterSkillCooldowns = { 30f, 30f, 30f };
    public float[] BossMonsterSkillTimers = new float[3];  // �� ��ų�� ���� ��Ÿ���� �����ϴ� �迭

    private float AllSkillCooldown = 5f;  // ��ü ��ų ��Ÿ��
    public float AllSkillCooldownTimer;  // ��ü ��ų ��Ÿ�� Ÿ�̸�

    // private float S1Speed = 0f;

    public bool isBossPatern = false;  // ���� ���� Ȱ��ȭ ����
    public bool isBossAtking = false;

    // public GameObject BossSkill1Obj; 

    public override void Start()
    {
        base.Start();  // �θ� Ŭ������ Start() ȣ��
        MaxHp = 200f;  // ü�� �ʱ�ȭ
        MonsterDmg = 10;  // ���� ������ �ʱ�ȭ
        attackRange = 4.0f;
        CurHp = MaxHp;  // ü�� ����
        isBossPatern = false;
        isBossAtking = false;
        animator = GetComponent<Animator>();
        // StartCoroutine(StartRotate());
    }

    private IEnumerator StartRotate()
    {
        // Transform BossSkilObjT = BossSkill1Obj.transform;
        // S1Speed = 0.2f;
        // while(S1Speed <= 100f)
        // {
        //     BossSkilObjT.transform.Rotate(0, -S1Speed, 0);
        //     yield return new WaitForSeconds(0f); 
        // }
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
        if (!isBossPatern)
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
                    GameObject castleObj = GameObject.FindWithTag("Castle");
                    CastlePos = castleObj.transform;
                    agent.SetDestination(CastlePos.position);
                }
                else
                {
                    agent.ResetPath();
                }
            }
            else
            {
                agent.ResetPath();

                if (!isBossPatern && !isBossAtking)
                {
                    isBossPatern = true;
                    // StartCoroutine(BossPaternStart());
                    StartCoroutine(BossSkill2());
                }
            }
        }
        else
        {
            // closestTarget = GetClosestTarget();
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

        if (AllSkillCooldownTimer <= 0f && !isBossPatern)
        {
            agent.ResetPath();
            isBossPatern = true;
            StartCoroutine(BossPaternStart());
        }
    }

    private Transform GetClosestTarget()
    {
        float closestSqrDistance = Mathf.Infinity;
        Transform closestTarget = null;

        string[] tags = { "Player" };

        foreach (string tag in tags)
        {
            GameObject[] targetsWithTag = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject targetObj in targetsWithTag)
            {
                Transform target = targetObj.transform;
                if (target == null) continue;

                float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;

                // 조건을 확인하여 가장 가까운 대상을 찾음

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
            case 0: // StartCoroutine(BossSkill2());
                StartCoroutine(BossSkill3());
                break;
            case 1:
                StartCoroutine(BossSkill4());
                break;
            case 2:
                StartCoroutine(BossSkill5());
                break;
            default:
                Debug.LogWarning("Invalid skill index.");
                break;
        }
    }

    private IEnumerator BossSkill2() // 영부가 줄때까지
    {
        isBossAtking = false;
        isBossPatern = false;
        yield break;
    }

    private IEnumerator BossSkill3()
    {
        animator.SetTrigger("BossSkill3");
        isBossAtking = true;
        while (isBossAtking)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Idle"))
            {
                isBossPatern = false;
                isBossAtking = false;
                GameObject bossSkillPrefab3 = PhotonNetwork.Instantiate("Additional/bossSkillPrefab3", transform.position, Quaternion.identity);
                BossMonsterSkillTimers[0] = BossMonsterSkillCooldowns[0];
                AllSkillCooldownTimer = AllSkillCooldown;
                yield break;
            }
            else
            {
                yield return null;
            }
        }
        yield break;
    }

    private IEnumerator BossSkill4()
    {
        BossMonsterSkillTimers[1] = BossMonsterSkillCooldowns[1];
        AllSkillCooldownTimer = AllSkillCooldown;
        isBossAtking = false;
        isBossPatern = false;
        yield break;
    }

    private IEnumerator BossSkill5()
    {
        BossMonsterSkillTimers[2] = BossMonsterSkillCooldowns[2];
        AllSkillCooldownTimer = AllSkillCooldown;
        isBossAtking = false;
        isBossPatern = false;
        yield break;
    }
    private IEnumerator BossPaternStart()
    {
        int selectedSkill = GetRandomSkill();

            if (selectedSkill != -1)
            {
                UseSkill(selectedSkill);
            }

        yield break;
    }
}
