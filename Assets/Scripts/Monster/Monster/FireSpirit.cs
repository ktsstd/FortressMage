using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireSpirit : MonsterAI
{   
    public bool StartAtking = false;
    public Transform closestTarget;

    public override void Start()
    {
        base.Start();
        attackRange = 3.0f; 
        attackCooldown = 8f;
        MonsterDmg = 20;
        MaxHp = 60f;
        Speed = 4f;
        defaultspped = Speed;
        StartAtking = false;
        CurHp = MaxHp;
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
                    if (!StartAtking)
                    {
                        agent.SetDestination(closestTarget.position);
                    }
                }
                else
                {
                    agent.ResetPath();

                    if (attackTimer <= 0f && !StartAtking)
                    {
                        animator.SetTrigger("StartAttack");
                        StartAtking = true;
                        StartCoroutine(FireStartAttack());
                    }
                }
            }
            else
            {
                agent.ResetPath();
                if (StartAtking)
                {
                    StopCoroutine(FireStartAttack());
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

    private IEnumerator FireStartAttack()
    {
        yield return new WaitForSeconds(0.5f);
        while(StartAtking)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Spirit of Fire_Move"))
            {
                Debug.Log("3");
                FireDamageTarget(closestTarget);
                StartAtking = false;
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

    private void FireDamageTarget(Transform CurTarget)
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
    }
}
