using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpirit : MonsterAI
{   
    Animator animator;
    public bool StartAtking = false;

    public override void Start()
    {
        base.Start();
        attackRange = 2.0f; 
        attackCooldown = 1.5f;
        MonsterDmg = 20;
        MaxHp = 20f;
        Speed = 1f;
        StartAtking = false;
        CurHp = MaxHp;
    }

    public override void Update()
    {
        Transform closestTarget = GetClosestTarget();

        if (closestTarget != null)
        {
            float sqrDistanceToTarget = (closestTarget.position - transform.position).sqrMagnitude;

            if (sqrDistanceToTarget > attackRange * attackRange)
            {
                agent.SetDestination(closestTarget.position);
            }
            else
            {
                agent.ResetPath();

                if (attackTimer <= 0f)
                {
                    animator.SetTrigger("StartAttack");
                    StartAtking = true;
                    // StartCoroutine(FireStartAttack());
                    // AttackTarget(closestTarget);
                    attackTimer = attackCooldown;
                }
            }
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

        string[] tags = { "skilltower", "turret", "Castle", "Player" };

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
                else if (tag == "Castle" || tag == "skilltower" || tag == "Player")
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
}
