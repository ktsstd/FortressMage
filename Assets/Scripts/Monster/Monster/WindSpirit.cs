using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSpirit : MonsterAI
{
    private Animator animator;
    private ParticleSystem particleSys; 
    private float stopDistance = 2.0f;
    private Transform closestTarget;
    public float speedBuff = 1.5f; // 버프 적용 시 속도 증가 비율
    public float attackCooldownReduction = 3f; // 쿨타임 감소량

    public override void Start()
    {
        base.Start();
        MaxHp = 400f;
        Speed = 5f;
        CurHp = MaxHp;
        animator = GetComponent<Animator>();
        particleSys = GetComponentInChildren<ParticleSystem>();
        MonsterDmg = 0;
    }

    public override void Update()
    {        
        closestTarget = GetClosestTarget();
        float distanceTotarget = Vector3.Distance(transform.position, closestTarget.position);

        if (distanceTotarget > attackRange + stopDistance)
        {
            agent.SetDestination(closestTarget.position);
        }

        // else if (distanceTotarget <= attackRange + stopDistance && distanceTotarget > attackRange)
        // {
        //     agent.ResetPath();
        //     StartAttack = true;
        //     StartCoroutine(LightAttackStart());
        // }

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

    private void OnTriggerEnter(Collider other)
    {
        MonsterAI monster = other.GetComponent<MonsterAI>();
        if (monster != null)
        {
            ApplyBuff(monster);
        }
    }

    // private void OnTriggerExit(Collider other)
    // {
    //     MonsterAI monster = other.GetComponent<MonsterAI>();
    //     if (monster != null)
    //     {
    //         RemoveBuff(monster);
    //     }
    // }

    private void ApplyBuff(MonsterAI monster)
    {
        monster.Speed *= speedBuff; // 몬스터 속도 증가
        monster.AttackCooldown -= attackCooldownReduction; // 공격 쿨타임 감소

        // 쿨타임이 0 미만이 되지 않도록 방지
        if (monster.AttackCooldown < 0)
        {
            monster.AttackCooldown = 0;
        }
    }

    // private void RemoveBuff(MonsterAI monster)
    // {
    //     monster.Speed /= speedBuff; // 원래 속도로 복구
    //     monster.AttackCooldown += attackCooldownReduction; // 원래 쿨타임으로 복구

    //     Debug.Log($"{monster.name}이 버프 장판에서 나갔다");
    // }

    public void MonsterDmged(int playerdamage)
    {
        if (CurHp <= 0)
        {
            CurHp -= playerdamage;
        }

        else
        {
            Destroy(this.gameObject);
        }
    }
}
