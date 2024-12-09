using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IceSpirit : MonsterAI
{
    public Transform closestTarget;
    public Transform projectilePos;

    private float projectileSpeed; // 발사체 속도 설정

    public bool StartAtking = false;
    
    private string projectilePrefab = "Projectile/Ice"; // 발사체 프리팹 참조

    // Start() 함수: 몬스터 초기 설정
    public override void Start()
    {
        base.Start(); // MonsterAI 스크립트의 Start() 함수 호출 (기본 AI 로직 유지)
        StartAtking = false;
        MaxHp = 60f;
        Speed = 3f; // 몬스터 이동 속도 설정
        defaultspped = Speed;
        AttackCooldown = 8.0f; // 공격 쿨타임 설정
        attackRange = 25.0f; // 공격 범위 설정
        projectileSpeed = 1.6f;
        CurHp = MaxHp;
    }

    // Update() 함수: 매 프레임마다 호출되는 함수
    public override void Update()
    {
        if (!StartAtking && !NoTarget)
        {
            closestTarget = GetClosestTarget();
        }
        
        if (closestTarget != null)
        {
            float sqrDistanceToTarget = (closestTarget.position - transform.position).sqrMagnitude;
            if (canMove)
            {
                if (sqrDistanceToTarget > attackRange * attackRange)
                {
                    if (!StartAtking)
                    {
                        agent.SetDestination(closestTarget.position);
                    }
                }
                else
                {
                    agent.ResetPath();

                    if (attackTimer <= 0f && !StartAtking)
                    {
                        animator.SetTrigger("StartAttack");
                        StartAtking = true;
                        StartCoroutine(IceStartAttack());
                    }
                }
            }
            else
            {
                agent.ResetPath();
                if (StartAtking)
                {
                    StopCoroutine(IceStartAttack());
                }
            }            
        }
        else
        {
            // closestTarget = GetClosestTarget();
            NoTarget = true;
            GameObject castleObj = GameObject.FindWithTag("Castle");
            closestTarget = castleObj.transform;
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

        string[] tags = { "skilltower", "turret", "Player" };

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
                if (tag == "Player")
                {
                    PlayerController playerScript = target.GetComponent<PlayerController>();
                    if (playerScript != null)
                    {
                        if (playerScript.isDie) continue;
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

    private IEnumerator IceStartAttack()
    {
        yield return new WaitForSeconds(1f);
        while(StartAtking)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (!stateInfo.IsName("Spirit of Ice_Idle+Walk"))
            {
                if (animTime >= 0.67f)
                {
                    StartAtking = false;
                    attackTimer = attackCooldown;
                    LaunchProjectile();
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

    private void LaunchProjectile()
    {
        // 발사체를 PhotonNetwork.Instantiate를 사용하여 생성
        GameObject projectile = PhotonNetwork.Instantiate(projectilePrefab, projectilePos.position, Quaternion.identity);

        // 생성된 발사체에서 Projectile 스크립트를 가져옴
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        // 발사체 스크립트가 있으면 초기화 진행
        if (projectileScript != null)
        {
            // 플레이어 위치, 발사체 속도, 몬스터 데미지를 설정하여 발사체 초기화
            projectileScript.Initialize(closestTarget.position, projectileSpeed, MonsterDmg);
        }
    }
}
