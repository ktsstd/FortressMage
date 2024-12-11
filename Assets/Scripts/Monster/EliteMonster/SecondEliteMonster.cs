using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SecondEliteMonster : MonsterAI
{
    public ParticleSystem[] particleSys;
    private Animator thisanimator;
    private Transform closestTarget;

    private bool StartAtking = false;
    private bool isEliteMonsterPatern = false;

    private float stopDistance = 20.0f;
    private float speedBuff = 2f; // 버프 적용 시 속도 증가 비율
    private float attackCooldownReduction = 4f; // 쿨타임 감소량
    private float[] EliteskillCooldown = { 10f, 10f, 10f };
    private float AllSkillCooldown = 10f;

    private float[] EliteSkillTimers = new float[3];
    public float AllSkillCooldownTimer;

    public override void Start()
    {
        base.Start();
        thisanimator = GetComponentInChildren<Animator>();
        isEliteMonsterPatern = false;
        MaxHp = 60f;
        Speed = 4f;
        attackRange = 1f;
        defaultspped = Speed;
        CurHp = MaxHp;
        MonsterDmg = 0;
    }
    
    public override void Update()
    {
        if (!StartAtking && !NoTarget)
        {
            closestTarget = GetClosestTarget();
        }
        if (closestTarget == null)
        {
            NoTarget = true;
            GameObject castleObj = GameObject.FindWithTag("Castle");
            closestTarget = castleObj.transform;
        }
        
        float distanceTotarget = Vector3.Distance(transform.position, closestTarget.position);
        if (canMove)
        {
            if (distanceTotarget > attackRange + stopDistance)
            {
                agent.SetDestination(closestTarget.position);
            }
            else
            {
                agent.ResetPath();
                if (AllSkillCooldownTimer <= 0 && !StartAtking)
                {
                    StartAtking = true;   
                    StartCoroutine(SecEliteMonsterChoosePattern());
                }
            }
        }

        for (int i = 0; i < EliteSkillTimers.Length; i++)
        {
            if (EliteSkillTimers[i] > 0f)
            {
                EliteSkillTimers[i] -= Time.deltaTime;
            }
        }

        AllSkillCooldownTimer = Mathf.Max(0f, AllSkillCooldownTimer - Time.deltaTime);

    }

    private Transform GetClosestTarget()
    {
        float closestSqrDistance = Mathf.Infinity;
        Transform closestTarget = null;

        string[] tags = { "skilltower", "turret", "Castle" };

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
                    if (towerScript != null && towerScript.canAttack)
                    {
                        if (sqrDistanceToTarget < closestSqrDistance)
                        {
                            closestSqrDistance = sqrDistanceToTarget;
                            closestTarget = target;
                        }
                    }
                }

                if (tag == "skilltower")
                {
                    Skilltower skilltowerScript = target.GetComponent<Skilltower>();
                    if (skilltowerScript != null && skilltowerScript.canAttack)
                    {
                        if (sqrDistanceToTarget < closestSqrDistance)
                        {
                            closestSqrDistance = sqrDistanceToTarget;
                            closestTarget = target;
                        }
                    }
                }

                if (tag == "Castle")
                {
                    if (sqrDistanceToTarget < closestSqrDistance)
                    {
                        closestSqrDistance = sqrDistanceToTarget;
                        closestTarget = target;
                    }
                }
            }
        }

        return closestTarget;
    }

    private int GetRandomSkill()
    {
        List<int> availableSkills = new List<int>();

        for (int i = 0; i < EliteSkillTimers.Length; i++)
        {
            if (EliteSkillTimers[i] <= 0f)
            {
                availableSkills.Add(i);
            }
        }

        if (availableSkills.Count > 0)
        {
            return availableSkills[Random.Range(0, availableSkills.Count)];
        }

        else
        {
            AllSkillCooldownTimer = AllSkillCooldown;
        }
        return -1; 
    }

    private IEnumerator SecEliteMonsterChoosePattern()
    {
        if (!isEliteMonsterPatern && PhotonNetwork.IsMasterClient)
        {
            isEliteMonsterPatern = true;
            int selectedSkill = GetRandomSkill();

            if (selectedSkill != -1)
            {
                photonView.RPC("UseSkill", RpcTarget.All, selectedSkill);
            }

        }
        
        yield break;
    }

    [PunRPC]
    public void UseSkill(int skillIndex)
    {
        switch (skillIndex)
        {
            case 0: 
                StartCoroutine(EliteMonsterHealStart());
                break;
            case 1:
                StartCoroutine(EliteMonsterStun());
                break;
            case 2:
                StartCoroutine(EliteMonsterBuff());
                break;
            default:
                Debug.LogWarning("Invalid skill index.");
                AllSkillCooldownTimer = AllSkillCooldown;
                break;
        }
    }

    private IEnumerator EliteMonsterHealStart()
    {
        MonsterAI[] monsters = FindObjectsOfType<MonsterAI>();
        thisanimator.SetTrigger("StartHeal");
        particleSys[0].Play();
        Vector3 soundPosition = transform.position;
        soundManager.PlayMonster(17, 0.4f, soundPosition);
        bool anyHealed = false;

        if (monsters.Length == 0)
        {
            isEliteMonsterPatern = false;
            StartAtking = false; 
            EliteSkillTimers[0] = EliteskillCooldown[0];
            AllSkillCooldownTimer = AllSkillCooldown;  
            yield break;
        }

        foreach (MonsterAI heal in monsters)
        {
            if (!heal.hasHealed)
            {
                float MonsterMaxHp = heal.MaxHp;
                float MonsterCurHp = heal.CurHp;
                float lostHp = MonsterMaxHp - MonsterCurHp;

                if (lostHp > 0)
                {
                    float healHp = lostHp * 0.05f;
                    float newCurHp = Mathf.Min(MonsterCurHp + healHp, MonsterMaxHp);

                    Debug.Log("몬스터: " + heal.gameObject.name);
                    Debug.Log("현재 체력: " + MonsterCurHp);
                    Debug.Log("최대 체력: " + MonsterMaxHp);
                    Debug.Log("회복해야 할 값: " + healHp);
                    Debug.Log("회복 후 남은 체력: " + newCurHp);

                    heal.CurHp = newCurHp;

                    heal.hasHealed = true;
                    anyHealed = true;
                }
            }
        }        
        
        if (anyHealed)
        {
            StartCoroutine(ResetHealAfterDelay(9f));
            isEliteMonsterPatern = false;
            StartAtking = false; 
            EliteSkillTimers[0] = EliteskillCooldown[0];
            AllSkillCooldownTimer = AllSkillCooldown;  
        }
        yield break;
    }

    private IEnumerator ResetHealAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        MonsterAI[] monsters = FindObjectsOfType<MonsterAI>();

        foreach (MonsterAI heal in monsters)
        {
            heal.hasHealed = false;
        }
    }

    private IEnumerator EliteMonsterStun()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        thisanimator.SetTrigger("StartStun");
        Vector3 soundPosition = transform.position;
        soundManager.PlayMonster(18, 1f, soundPosition);
        particleSys[1].Play();
        if (players.Length == 0)
        {
            isEliteMonsterPatern = false;
            StartAtking = false; 
            EliteSkillTimers[1] = EliteskillCooldown[1];
            AllSkillCooldownTimer = AllSkillCooldown;  
            yield break;
        }
        foreach (PlayerController playerObj in players)
        {
            playerObj.OnPlayerStun(1.2f);
        }
        EliteSkillTimers[1] = EliteskillCooldown[1];
        AllSkillCooldownTimer = AllSkillCooldown;  
        isEliteMonsterPatern = false;
        StartAtking = false;
        yield break;
    }

    private IEnumerator EliteMonsterBuff()
    {
        MonsterAI[] monsters = FindObjectsOfType<MonsterAI>();
        thisanimator.SetTrigger("StartBuff");
        particleSys[2].Play();
        Vector3 soundPosition = transform.position;
        soundManager.PlayMonster(19, 1f, soundPosition);
        if (monsters.Length == 0)
        {
            isEliteMonsterPatern = false;
            StartAtking = false; 
            EliteSkillTimers[1] = EliteskillCooldown[1];
            AllSkillCooldownTimer = AllSkillCooldown;  
            yield break;
        }
        foreach (MonsterAI monsterObj in monsters)
        {
            if (!monsterObj.hasBuffed)
            {
                if (monsterObj.isSlow)
                {
                    float SlowedSpeed = monsterObj.defaultspped;
                    SlowedSpeed -= monsterObj.Speed;
                    monsterObj.Speed = (monsterObj.defaultspped * speedBuff) - SlowedSpeed;
                }
                else
                {
                    monsterObj.Speed *= speedBuff;
                }
                monsterObj.AttackCooldown -= attackCooldownReduction;
                monsterObj.hasBuffed = true;
            }
            if (monsterObj.AttackCooldown < 0)
            {
                monsterObj.AttackCooldown = 0;
            }
        }
        EliteSkillTimers[2] = EliteskillCooldown[2];
        AllSkillCooldownTimer = AllSkillCooldown;  
        StartAtking = false;
        isEliteMonsterPatern = false;
        yield break;
    }
}
