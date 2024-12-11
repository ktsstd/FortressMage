using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FirstEliteMonster : MonsterAI
{
    private int MonsterShield = 60;
    private int CurShield = 0;
    private float MaxHp40Per;
    private float stopDistance = 7f;

    private bool StartAtking = false;
    private bool isShielded = false;

    private Transform closestTarget;

    public ParticleSystem[] ParticleSys;    

    public override void Start()
    {
        base.Start();
        attackCooldown = 5.0f;
        attackTimer = attackCooldown; 
        attackRange = 5.0f;    
        MaxHp = 200f;
        CurHp = MaxHp;
        MaxHp40Per = MaxHp * 0.4f;
        MonsterDmg = 10;
    }

    public override void Update()
    {
        if (!StartAtking && !NoTarget)
        {
            closestTarget = GetClosestTarget();
        }
        
        if (closestTarget != null)
        {
            float sqrDistanceToTarget = (closestTarget.position - transform.position).sqrMagnitude;
            if (canMove)
            {
                if (sqrDistanceToTarget > attackRange * attackRange)
                {
                    if (!closestTarget.CompareTag("Obstacle"))
                    {
                        if (!StartAtking)
                        {
                            animator.SetBool("StartMove", true);
                            agent.SetDestination(closestTarget.position);
                        }
                    }
                    else
                    {
                        if (!StartAtking)
                        {
                            agent.ResetPath();
                            StartAtking = true;
                            animator.SetBool("StartMove", false);
                            animator.SetBool("EliteSkill2", true);
                            StartCoroutine(EliteMonster1Skill2());
                        }
                    }
                }
                else
                {
                    agent.ResetPath();
                    if (!StartAtking && attackTimer <= 0f)
                    {
                        StartAtking = true;
                        animator.SetBool("StartMove", false);
                        animator.SetTrigger("EliteSkill1");
                        StartCoroutine(EliteMonster1Skill1());
                    }
                }
            }
            else
            {
                agent.ResetPath();
                if (StartAtking)
                {
                    animator.SetBool("StartMove", false);
                    StopCoroutine(EliteMonster1Skill1());
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

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
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
                    StoneWall stonewallScript = target.GetComponent<StoneWall>();
                    if (stonewallScript != null)
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

        return closestTarget;
    }

    private void EliteDamageTarget(Transform CurTarget)
    {
        if (CurTarget.CompareTag("skilltower"))
        {
            Skilltower skillTowerScript = CurTarget.GetComponent<Skilltower>();
            if (skillTowerScript != null)
            {
                skillTowerScript.TakeDamage(MonsterDmg);
            }
        }

        if (CurTarget.CompareTag("Castle"))
        {
            Wall castleScript = CurTarget.GetComponent<Wall>();
            if (castleScript != null)
            {
                castleScript.TakeDamage(MonsterDmg);
            }
        }

        if (CurTarget.CompareTag("turret"))
        {
            Turret towerScript = CurTarget.GetComponent<Turret>();
            if (towerScript != null)
            {
                towerScript.TakeDamage(MonsterDmg);
            }
        }

        if (CurTarget.CompareTag("Player"))
        {
            PlayerController playerScript = CurTarget.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                playerScript.OnHitPlayer(MonsterDmg);
            }
        }

        if (CurTarget.CompareTag("Obstacle"))
        {
            StoneWall obStacleScript = CurTarget.GetComponent<StoneWall>();
            if (obStacleScript != null)
            {
                obStacleScript.SelfDestroy();
            }
        }
    }

    private IEnumerator EliteMonster1Skill1()
    {
        Vector3 soundPosition = transform.position;
        yield return new WaitForSeconds(0.5f);
        while(StartAtking)
        {
            float animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            // AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            // if (stateInfo.IsName("Idle"))
            if (animTime >= 0.8f)
            {
                soundManager.PlayMonster(4, 1.0f, soundPosition);
                yield return new WaitForSeconds(0.2f);
                StartAtking = false;
                ParticleSys[0].transform.position = closestTarget.position;
                ParticleSys[0].Play();
                EliteDamageTarget(closestTarget);
                attackTimer = attackCooldown;
                yield break;
            }

            else
            {
                yield return null;
            }
        }
        yield break;
    }

    private IEnumerator EliteMonster1Skill2()
    {
        bool isPlaying = false;
        defaultspped = Speed;
        Speed = 12f;
        yield return new WaitForSeconds(0.5f);
        while(StartAtking)
        {
            if (!canMove)
            {
                agent.ResetPath();
                yield return null;
                continue;
            }
            float sqrDistanceToTarget = (closestTarget.position - transform.position).sqrMagnitude;
            Vector3 soundPosition = closestTarget.position;
            
            if (sqrDistanceToTarget > attackRange + stopDistance)
            {
                if (closestTarget.CompareTag("Obstacle"))
                {
                    soundClip.clip = MonsterAudio[0];
                    soundClip.loop = true;
                    if (!isPlaying)
                    {
                        soundClip.Play();
                        isPlaying = true;
                    }
                    agent.SetDestination(closestTarget.position);
                }
                else
                {
                    agent.ResetPath();
                    attackTimer = 0.5f;
                    Speed = defaultspped;
                    animator.SetBool("EliteSkill2", false);
                    StartAtking = false;
                    soundClip.Stop();
                }
            }
            else
            {
                agent.velocity = Vector3.zero;
                soundManager.PlayMonster(5, 1.0f, soundPosition);
                ParticleSys[1].transform.position = closestTarget.position;
                ParticleSys[1].Play();
                agent.ResetPath();
                EliteDamageTarget(closestTarget);
                attackTimer = 0.5f;
                Speed = defaultspped;
                animator.SetBool("EliteSkill2", false);
                StartAtking = false;
                soundClip.Stop();
                yield break;
            }
            yield return null;
        }
        yield break;
    }

    private IEnumerator EliteMonsterShield(int MonsterShield)
    {
        agent.ResetPath();
        isShielded = true;
        ParticleSys[2].Play();
        CurShield = MonsterShield;
        // animator.SetTrigger("EltieSkill1");
        yield return new WaitForSeconds(2f); // 2초 대기
        Debug.Log("Shield");
        
    }

    public override void MonsterDmged(int playerdamage)
    {
        if (!photonView.IsMine) return;
        if (CurShield >=0)
        {
            CurShield -= playerdamage;
            if (CurShield <= 0)
            {
                ParticleSys[2].Stop();
            }
        }

        else
        {
            CurHp -= playerdamage;
            if (CurHp <= MaxHp40Per && !isShielded)
            {
                StartCoroutine(EliteMonsterShield(MonsterShield));
            }
        }
        
        if (CurHp <= 0)
        {
            MonsterDied();
        }
    }
}