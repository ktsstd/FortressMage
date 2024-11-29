using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class DarkSpirit : MonsterAI, IPunObservable
{
    private Animator animator;
    private ParticleSystem particleSys;

    private Transform closestTarget;

    private float stopDistance = 8.0f;

    public bool Attacked = false;

    private Vector3 StartPosition;  // 몬스터의 초기 위치

    public override void Start()
    {
        base.Start();
        MaxHp = 400f;
        Speed = 10f;
        attackRange = 3f;
        CurHp = MaxHp;
        animator = GetComponent<Animator>();
        particleSys = GetComponentInChildren<ParticleSystem>();
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
            if (!Attacked)
            {
                animator.SetBool("StartMove", true);
                agent.SetDestination(closestTarget.position);
            }
        }
        else
        {
            if (!Attacked)
            {
                animator.SetBool("StartMove", false);
                agent.ResetPath();
                StartCoroutine(DarkAttackStart());
            }
        }

        if (Vector3.Distance(transform.position, StartPosition) <= stopDistance && !agent.pathPending)
        {
            Attacked = false;
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
        Attacked = true;
        animator.SetTrigger("StartAttack");
        yield return new WaitForSeconds(1.5f);
        DarkDamageTarget(closestTarget);
        animator.SetBool("StartMove", true);
        agent.SetDestination(StartPosition);
        yield break;
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
