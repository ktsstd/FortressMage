using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSpirit : MonsterAI
{
    public ParticleSystem particleSys;
    private Transform closestTarget;

    private bool StartAtking = false;

    private float stopDistance = 20.0f;
    private float skillCooldown = 6f;
    public float skillCooltime;

    public override void Start()
    {
        base.Start();
        MaxHp = 60f;
        Speed = 4f;
        attackRange = 1f;
        defaultspped = Speed;
        CurHp = MaxHp;
        particleSys = GetComponentInChildren<ParticleSystem>();
        MonsterDmg = 0;
    }

    public override void Update()
    {
        if (!NoTarget)
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
                if (skillCooltime <= 0 && !StartAtking)
                {
                    StartAtking = true;
                    particleSys.Play();       
                    animator.SetTrigger("StartAttack");
                    StartCoroutine(WindStartAttack());
                }
            }
        }
        

        skillCooltime = Mathf.Max(0f, skillCooltime - Time.deltaTime);

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

                else if (tag == "Castle" || tag == "skilltower")
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

    private IEnumerator WindStartAttack()
    {

        yield return new WaitForSeconds(0.5f);
        while(StartAtking)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            Vector3 soundPosition = transform.position;
            if (!stateInfo.IsName("Idle"))
            {
                if (animTime >= 0.4f)
                {
                    StartAtking = false;
                    skillCooltime = skillCooldown;
                    yield return new WaitForSeconds(0.4f);
                    soundManager.PlayMonster(8, 1.0f, soundPosition);
                    yield break;
                }
                else
                {
                    yield return null;
                }
            }
        }
        yield break;
    }
}
