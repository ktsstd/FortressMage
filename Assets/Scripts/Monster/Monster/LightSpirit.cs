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
        MaxHp = 400f;
        Speed = 5f;
        CurHp = MaxHp;
        StartAttack = false;
        animator = GetComponent<Animator>();
        particleSys = GetComponentInChildren<ParticleSystem>();
        MonsterDmg = 50;
        // GameObject castleObject = GameObject.FindWithTag("Castle");
        // if (castleObject != null)
        // {
        //     castleTransform = castleObject.transform;
        // }

        // else
        // {
        //     return;
        // }
    }

    public override void Update()
    {
        // if (castleTransform == null) return;
        // if (StartAttack) return;
        bool isAtking = animator.GetBool("StartAttack");
        
        if (!isAtking && !StartAttack)
        {
            closestTarget = GetClosestTarget();
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


    private IEnumerator LightAttackStart()
    {
        yield return new WaitForSeconds(3f);
        animator.SetBool("StartAttack", true);
        particleSys.Play();
        yield return new WaitForSeconds(2f);
        LightDamageTarget(closestTarget);
        yield return new WaitForSeconds(2f);
        photonView.RPC("MonsterDied", RpcTarget.All);
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
