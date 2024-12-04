using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class LightSpirit : MonsterAI, IPunObservable
{
    private Animator animator;
    // private Transform castleTransform;
    private ParticleSystem particleSys;
    
    private Transform closestTarget;

    private float stopDistance = 2.0f;
    // private float fadeDuration = 8.0f;

    private bool StartAttack = false;

    public override void Start()
    {
        base.Start();
        MaxHp = 60f;
        Speed = 5f;
        defaultspped = Speed;
        CurHp = MaxHp;
        StartAttack = false;
        animator = GetComponent<Animator>();
        particleSys = GetComponentInChildren<ParticleSystem>();
        MonsterDmg = 50;
    }

    public override void Update()
    {
        bool isAtking = animator.GetBool("StartAttack");
        if (!isAtking && !StartAttack)
        {
            if (!NoTarget)
            {
                closestTarget = GetClosestTarget();
            }

            else
            {
                NoTarget = true;
                GameObject castleObj = GameObject.FindWithTag("Castle");
                closestTarget = castleObj.transform;
            }
            float distanceTotarget = Vector3.Distance(transform.position, closestTarget.position);

            if (distanceTotarget > attackRange + stopDistance)
            {
                agent.SetDestination(closestTarget.position);
            }
            else if (distanceTotarget <= attackRange + stopDistance && distanceTotarget > attackRange)
            {
                agent.ResetPath();
                StartAttack = true;
                StartCoroutine(LightAttackStart());
            }
        }

        else
        {
            agent.ResetPath();
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
        while(StartAttack)
        {
            yield return new WaitForSeconds(3f);
            animator.SetBool("StartAttack", true);
            particleSys.Play();
            Animator LightAnim = GetComponent<Animator>();
            // AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float animTime = LightAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (animTime >= 1.0f)
            {
                LightDamageTarget(closestTarget);
                photonView.RPC("MonsterDied", RpcTarget.All);
            }
            // if (stateInfo.IsName("Spirit of Light_Animation_Move"))
            // {
            //     LightDamageTarget(closestTarget);
            //     photonView.RPC("MonsterDied", RpcTarget.All);
            // }

            else
            {
                yield return null;
            }
        }
        // yield return new WaitForSeconds(3f);
        // animator.SetBool("StartAttack", true);
        // particleSys.Play();
        // yield return new WaitForSeconds(2f);
        // LightDamageTarget(closestTarget);
        // yield return new WaitForSeconds(2f);
        // photonView.RPC("MonsterDied", RpcTarget.All);
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
