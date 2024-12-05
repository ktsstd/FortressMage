using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Boss : MonsterAI
{
    private Transform closestTarget;
    private Transform CastlePos;
    private Transform ClosetPlayerpos;
    public Transform Boss4Pos1;
    public Transform Boss4Pos2;
    public Transform Boss4Pos3;
    public Transform Boss4Posx1;
    public Transform Boss4Posx2;

    // private ParticleSystem

    private float[] BossMonsterSkillCooldowns = { 30f, 5f, 30f };
    public float[] BossMonsterSkillTimers = new float[3];  // �� ��ų�� ���� ��Ÿ���� �����ϴ� �迭

    private float AllSkillCooldown = 5f;  // ��ü ��ų ��Ÿ��
    public float AllSkillCooldownTimer;  // ��ü ��ų ��Ÿ�� Ÿ�̸�
    private float BossObjDmg = 9999f;

    // private float S1Speed = 0f;

    public bool isBossPatern = false;  // ���� ���� Ȱ��ȭ ����
    public bool isBossAtking = false;
    public bool isBossUseSkill2 = false;

    public GameObject BossSkill2Obj;

    public override void Start()
    {
        MaxHp = 200f;
        base.Start();  // �θ� Ŭ������ Start() ȣ��
        MonsterDmg = 30;  // ���� ������ �ʱ�ȭ
        attackRange = 6.0f;
        Speed = 1.0f;
        isBossPatern = false;
        isBossAtking = false;
        isBossUseSkill2 = false;
        BossSkill2Obj.SetActive(false);
    }

    public override void Update()
    {
        closestTarget = GetClosestTarget();
        
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

                if (!isBossPatern && !isBossAtking && !isBossUseSkill2)
                {
                    if(closestTarget.CompareTag("Player"))
                    {
                        isBossPatern = true;
                        isBossUseSkill2 = true;
                        StartCoroutine(BossSkill2());
                    }
                    else
                    {
                        BossDamageTarget(closestTarget);
                    }
                }
            }
        }
        else
        {
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

        if (AllSkillCooldownTimer <= 0f)
        {
            agent.ResetPath();
            StartCoroutine(BossPaternStart());
        }
    }

    private Transform GetClosestTarget()
    {
        float closestSqrDistance = Mathf.Infinity;
        Transform closestTarget = null;

        string[] tags = { "skilltower", "turret", "Player", "Obstacle" };

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

                if (tag == "Obstacle")
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

    private GameObject FindPlayerpos()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject playerPos = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject playerObj in players)
        {
            float distancetoPlayer = Vector3.Distance(currentPosition, playerObj.transform.position);
            if (distancetoPlayer < shortestDistance)
            {
                shortestDistance = distancetoPlayer;
                playerPos = playerObj;
            }
        }
        return playerPos;
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

        else
        {
            AllSkillCooldownTimer = AllSkillCooldown;
            isBossPatern = false;
        }
        return -1;  // ����� �� �ִ� ��ų�� ���ٸ� -1 ��ȯ
    }

    [PunRPC]
    public int spawnPlaceint()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int spawnPlaceRandom = Random.Range(0, 3);
            return spawnPlaceRandom;
        }
        return -1;
    }

    [PunRPC]
    public void UseSkill(int skillIndex)
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
                AllSkillCooldownTimer = AllSkillCooldown;
                break;
        }
    }

    private IEnumerator BossSkill2()
    {
        animator.SetTrigger("BossSkill2");
        isBossAtking = true;
        yield return new WaitForSeconds(0.5f);
        while (isBossAtking)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Idle"))
            {
                isBossPatern = false;
                BossSkill2Obj.SetActive(true); 
                StartCoroutine(BossSkill2Start());
                // GameObject bossSkillPrefab3 = PhotonNetwork.Instantiate("Additional/bossSkillPrefab2", transform.position, Quaternion.identity);
                yield break;
            }
            else
            {
                yield return null;
            }
        }
        yield break;
    }
    
    private IEnumerator BossSkill2Start()
    {
        Animator bossskill2Anim = BossSkill2Obj.GetComponent<Animator>();
        yield return new WaitForSeconds(0.5f);
        while (BossSkill2Obj.activeSelf)
        {
            float animTime = bossskill2Anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (animTime >= 1.0f)
            {
                BossSkill2Obj.SetActive(false);
                isBossAtking = false;
                isBossUseSkill2 = false;
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator BossSkill3()
    {
        animator.SetTrigger("BossSkill3");
        isBossAtking = true;
        yield return new WaitForSeconds(0.5f);
        while (isBossAtking)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Idle"))
            {
                isBossPatern = false;
                isBossAtking = false;
                PhotonNetwork.Instantiate("Additional/bossSkillPrefab3", transform.position, Quaternion.identity);
                BossMonsterSkillTimers[0] = BossMonsterSkillCooldowns[0];
                AllSkillCooldownTimer = AllSkillCooldown;
                // yield return new WaitForSeconds(1f);

                
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
        animator.SetTrigger("BossSkill4");
        isBossAtking = true;
        yield return new WaitForSeconds(0.5f);
        while (isBossAtking)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Idle"))
            {
                isBossPatern = false;
                isBossAtking = false;
                int random = spawnPlaceint();
                if (random == 0)
                {
                    PhotonNetwork.Instantiate("Additional/Boss_Skill_4", Boss4Pos1.position, Quaternion.identity);
                    PhotonNetwork.Instantiate("Additional/Boss_Skill_4", Boss4Pos2.position, Quaternion.identity);
                    PhotonNetwork.Instantiate("Additional/Boss_Skill_4", Boss4Posx1.position, Boss4Posx1.rotation);
                    PhotonNetwork.Instantiate("Additional/Boss_Skill_4", Boss4Posx2.position, Boss4Posx2.rotation);
                }
                if (random == 1)
                {
                    PhotonNetwork.Instantiate("Additional/Boss_Skill_4", Boss4Pos1.position, Quaternion.identity);
                    PhotonNetwork.Instantiate("Additional/Boss_Skill_4", Boss4Pos3.position, Quaternion.identity);
                    PhotonNetwork.Instantiate("Additional/Boss_Skill_4", Boss4Posx1.position, Boss4Posx1.rotation);
                    PhotonNetwork.Instantiate("Additional/Boss_Skill_4", Boss4Posx2.position, Boss4Posx2.rotation);
                }
                if (random == 2)
                {
                    PhotonNetwork.Instantiate("Additional/Boss_Skill_4", Boss4Pos2.position, Quaternion.identity);
                    PhotonNetwork.Instantiate("Additional/Boss_Skill_4", Boss4Pos3.position, Quaternion.identity);
                    PhotonNetwork.Instantiate("Additional/Boss_Skill_4", Boss4Posx1.position, Boss4Posx1.rotation);
                    PhotonNetwork.Instantiate("Additional/Boss_Skill_4", Boss4Posx2.position, Boss4Posx2.rotation);
                }
                // GameObject bossSkillPrefab4 = PhotonNetwork.Instantiate("Additional/Boss_Skill_4", transform.position, Quaternion.identity);
                BossMonsterSkillTimers[1] = BossMonsterSkillCooldowns[1];
                AllSkillCooldownTimer = AllSkillCooldown;
                // yield return new WaitForSeconds(1f);

                
                yield break;
            }
            else
            {
                yield return null;
            }
        }
        yield break;
    }

    private IEnumerator BossSkill5()
    {
        animator.SetTrigger("BossSkill5");
        isBossAtking = true;
        yield return new WaitForSeconds(0.5f);
        GameObject playerPos = FindPlayerpos();
        ClosetPlayerpos = playerPos.transform; 
        while (isBossAtking)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Idle"))
            {
                isBossPatern = false;
                isBossAtking = false;
                PhotonNetwork.Instantiate("Additional/Boss_Skill_5", ClosetPlayerpos.position, Quaternion.Euler(-90, 0, 0));
                BossMonsterSkillTimers[2] = BossMonsterSkillCooldowns[2];
                AllSkillCooldownTimer = AllSkillCooldown;
                // yield return new WaitForSeconds(1f);

                
                yield break;
            }
            else
            {
                yield return null;
            }
        }
        yield break;
    }
    private IEnumerator BossPaternStart()
    {
        if (!isBossPatern && PhotonNetwork.IsMasterClient)
        {
            isBossPatern = true;
            int selectedSkill = GetRandomSkill();

            if (selectedSkill != -1)
            {
                photonView.RPC("UseSkill", RpcTarget.All, selectedSkill);
                // UseSkill(selectedSkill);
            }

        }
        
        yield break;
    }

    private void BossDamageTarget(Transform CurTarget)
    {
        if (CurTarget.CompareTag("skilltower"))
        {
            Skilltower skillTowerScript = CurTarget.GetComponent<Skilltower>();
            if (skillTowerScript != null)
            {
                skillTowerScript.TakeDamage(BossObjDmg);
            }
        }

        if (CurTarget.CompareTag("Castle"))
        {
            Wall castleScript = CurTarget.GetComponent<Wall>();
            if (castleScript != null)
            {
                castleScript.TakeDamage(BossObjDmg);
            }
        }

        if (CurTarget.CompareTag("turret"))
        {
            Turret towerScript = CurTarget.GetComponent<Turret>();
            if (towerScript != null)
            {
                towerScript.TakeDamage(BossObjDmg);
            }
        }

        if (CurTarget.CompareTag("Obstacle"))
        {
            Destroy(CurTarget.gameObject);
        }

        // if (CurTarget.CompareTag("Player"))
        // {
        //     PlayerController playerScript = CurTarget.GetComponent<PlayerController>();
        //     if (playerScript != null)
        //     {
        //         playerScript.OnHitPlayer(MonsterDmg);
        //     }
        // }
    }

    public override void OnMonsterStun(float _time)
    {
        
    }
    public override void OnMonsterSpeedDown(float _time, float _moveSpeed)
    {
        
    }
}
