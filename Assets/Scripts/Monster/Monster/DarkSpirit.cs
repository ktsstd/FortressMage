using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class DarkSpirit : MonsterAI, IPunObservable
{
    // private ParticleSystem particleSys;

    private Transform closestTarget;

    private float stopDistance = 5.0f;

    private bool Attacked = false;

    private Vector3 StartPosition;  // 몬스터의 초기 위치

    public override void Start()
    {
        base.Start();
        MaxHp = 600f;
        Speed = 10f;
        defaultspped = Speed;
        attackRange = 3f;
        CurHp = MaxHp;
        Attacked = false;
        MonsterDmg = 50;

        StartPosition = transform.position;  // 몬스터의 초기 위치를 저장
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            OnMonsterStun(5f);
        }
        if (!Attacked && !NoTarget)
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
        if (canMove && !Attacked)
        {
            if (distanceTotarget > attackRange + stopDistance)
            {
                animator.SetTrigger("StartMove");
                agent.SetDestination(closestTarget.position);
            }

            else
            {
                Attacked = true;
                agent.ResetPath();
                agent.velocity = Vector3.zero;
                StartCoroutine(DarkAttackStart());
            }
        }
        else if(!canMove)
        {
            agent.ResetPath();
            if (Attacked)
            {
                StopCoroutine(DarkAttackStart());
            }
        }
    }

    private Transform GetClosestTarget()
    {
        float closestSqrDistance = Mathf.Infinity;
        Transform closestTarget = null;

        string[] tags = { "skilltower", "turret" };

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

    private IEnumerator DarkAttackStart()
    {
        animator.SetTrigger("StartAttack");
        while(Attacked)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Spirit of dark_Idle"))
            {
                DarkDamageTarget(closestTarget);
                // animator.SetBool("StartAttack", false);
                transform.position = StartPosition;
                Attacked = false;
                yield break;
            }

            else
            {
                yield return null;
            }
        }
    }

    private void DarkDamageTarget(Transform CurTarget)
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
    }
}
