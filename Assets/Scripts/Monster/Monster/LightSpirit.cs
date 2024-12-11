using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class LightSpirit : MonsterAI, IPunObservable
{
    // private Transform castleTransform;
    private ParticleSystem particleSys;
    private Transform closestTarget;

    private float stopDistance = 2.0f;
    // private float fadeDuration = 8.0f;

    private bool StartAtking = false;

    public override void Start()
    {
        base.Start();
        MaxHp = 30f;
        Speed = 5f;
        defaultspped = Speed;
        CurHp = MaxHp;
        StartAtking = false;
        particleSys = GetComponentInChildren<ParticleSystem>();
        MonsterDmg = 20;
    }

    public override void Update()
    {

        if (!StartAtking && !NoTarget)
        {
            closestTarget = GetClosestTarget();
        }

        if (closestTarget != null)
        {
            float distanceTotarget = Vector3.Distance(transform.position, closestTarget.position);
            if (canMove)
            {
                if (distanceTotarget > attackRange + stopDistance)
                {
                    if (!StartAtking)
                    {
                        agent.SetDestination(closestTarget.position);
                    }
                }
                else
                {
                    agent.ResetPath();
                    if (!StartAtking)
                    {
                        animator.SetTrigger("StartAttack");
                        StartAtking = true;
                        StartCoroutine(LightAttackStart());
                    }
                }
            }
            else
            {
                agent.ResetPath();
                if (StartAtking)
                {
                    StopCoroutine(LightAttackStart());
                }
            }
        }
        else
        {
            NoTarget = true;
            GameObject castleObj = GameObject.FindWithTag("Castle");
            closestTarget = castleObj.transform;
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

    private IEnumerator LightAttackStart()
    {
        Vector3 soundPosition = transform.position;
        bool soundPlayed1 = false;
        bool soundPlayed2 = false;
        yield return new WaitForSeconds(0.5f);
        while(StartAtking)
        {
            particleSys.Play();
            if (!soundPlayed1)
            {
                soundManager.PlayMonster(2, 1.0f, soundPosition);
                soundPlayed1 = true;
            }
            float animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (animTime >= 0.63f && !soundPlayed2)
            {
                soundPosition = transform.position;
                soundManager.PlayMonster(3, 1.0f, soundPosition);
                soundPlayed2 = true;
                yield return new WaitForSeconds(0.2f);
                LightDamageTarget(closestTarget);
                yield return new WaitForSeconds(1f);
                photonView.RPC("MonsterDied", RpcTarget.All);
            }
            else
            {
                yield return null;
            }
        }
        yield break;
    }

    private void LightDamageTarget(Transform CurTarget)
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
