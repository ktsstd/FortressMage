using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Unity.VisualScripting;

public class DarkSpirit : MonsterAI, IPunObservable
{
    private Animator animator;
    // private ParticleSystem particleSys;

    public Transform closestTarget;

    private float stopDistance = 8.0f;

    private bool Attacked = false;

    private Vector3 StartPosition;  // 몬스터의 초기 위치

    public override void Start()
    {
        base.Start();
        MaxHp = 400f;
        Speed = 10f;
        attackRange = 3f;
        CurHp = MaxHp;
        Attacked = false;
        animator = GetComponent<Animator>();
        MonsterDmg = 50;

        StartPosition = transform.position;  // 몬스터의 초기 위치를 저장
    }

    public override void Update()
    {
        if (!Attacked)
        {
            closestTarget = GetClosestTarget();
        }
        float distanceTotarget = Vector3.Distance(transform.position, closestTarget.position);

        if (distanceTotarget > attackRange + stopDistance)
        {
            animator.SetBool("StartMove", true);
            agent.SetDestination(closestTarget.position);
        }

        else
        {
            bool isStartAttack = animator.GetBool("StartAttack");
            if (!isStartAttack) // 공격시작
            {
                animator.SetBool("StartAttack", true);
                animator.SetBool("StartMove", false);
                Attacked = true;
                agent.ResetPath();
                StartCoroutine(DarkAttackStart());
            }
        }
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

                if (target.CompareTag("skilltower"))
                {
                    float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;
                    if (sqrDistanceToTarget < closestSqrDistance)
                    {
                        closestSqrDistance = sqrDistanceToTarget;
                        closestTarget = target;
                    }
                    // Skilltower skillTowerScript = CurTarget.GetComponent<Skilltower>();
                    // if (skillTowerScript.canAttack == true)
                    // {
                    //     float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;
                    //     if (sqrDistanceToTarget < closestSqrDistance)
                    //     {
                    //         closestSqrDistance = sqrDistanceToTarget;
                    //         closestTarget = target;
                    //     }
                    // }
                }

                if (target.CompareTag("turret"))
                {
                    Turret towerScript = target.GetComponent<Turret>();
                    if (towerScript.canAttack == true)
                    {
                        float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;
                        if (sqrDistanceToTarget < closestSqrDistance)
                        {
                            closestSqrDistance = sqrDistanceToTarget;
                            closestTarget = target;
                        }
                    }
                }

                if (target.CompareTag("Castle"))
                {
                    float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;
                    if (sqrDistanceToTarget < closestSqrDistance)
                    {
                        closestSqrDistance = sqrDistanceToTarget;
                        closestTarget = target;
                    }
                    // Wall castleScript = CurTarget.GetComponent<Wall>();
                    // if (castleScript.canAttack == true)
                    // {
                    //     float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;
                    //     if (sqrDistanceToTarget < closestSqrDistance)
                    //     {
                    //         closestSqrDistance = sqrDistanceToTarget;
                    //         closestTarget = target;
                    //     }
                    // }
                }

                // float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;

                // if (sqrDistanceToTarget < closestSqrDistance)
                // {
                //     closestSqrDistance = sqrDistanceToTarget;
                //     closestTarget = target;
                // }
            }
        }

        return closestTarget;
    }

    private IEnumerator DarkAttackStart()
    {
        while(Attacked)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Spirit of dark_Idle"))
            {
                DarkDamageTarget(closestTarget);
                Attacked = false;
                animator.SetBool("StartMove", true);
                animator.SetBool("StartAttack", false);
                transform.position = StartPosition;
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
