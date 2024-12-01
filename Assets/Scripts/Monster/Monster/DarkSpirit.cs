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
    private bool isMoveToStart = false;

    private Vector3 StartPosition;  // 몬스터의 초기 위치

    public override void Start()
    {
        base.Start();
        MaxHp = 400f;
        Speed = 10f;
        attackRange = 3f;
        CurHp = MaxHp;
        Attacked = false;
        isMoveToStart = false;
        animator = GetComponent<Animator>();
        // particleSys = GetComponentInChildren<ParticleSystem>();
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
            if (!Attacked) // 타겟으로 이동
            {
                if (!isMoveToStart)
                {
                    animator.SetBool("StartMove", true);
                    agent.SetDestination(closestTarget.position);
                }
            }
        }
        else
        {
            bool isStartAttack = animator.GetBool("StartAttack");
            if (!isStartAttack && !isMoveToStart) // 공격시작
            {
                animator.SetBool("StartAttack", true);
                animator.SetBool("StartMove", false);
                Attacked = true;
                agent.ResetPath();
                StartCoroutine(DarkAttackStart());
            }
        }

        if (Vector3.Distance(transform.position, StartPosition) <= stopDistance && !agent.pathPending)
        {
            isMoveToStart = false;
        }

        else
        {
            if (isMoveToStart)
            {
                agent.SetDestination(StartPosition);
            }
        }

        // bool isStartMove = animator.GetBool("StartMove");
        // if (Attacked && isStartMove)
        // {
        //     agent.SetDestination(StartPosition);
        // }

        // if (Vector3.Distance(transform.position, StartPosition) <= stopDistance && !agent.pathPending) // 시작지점에 있는가
        // {
        //     agent.SetDestination(closestTarget.position);
        //     Attacked = false;
        // }
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

                if (sqrDistanceToTarget < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistanceToTarget;
                    closestTarget = target;
                }
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
                isMoveToStart = true;
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
        Debug.Log("2");
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
            Debug.Log("3");
            Turret towerScript = CurTarget.GetComponent<Turret>();
            if (towerScript != null)
            {
                towerScript.TakeDamage(MonsterDmg);
            }
        }
    }
}
