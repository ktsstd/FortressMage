using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IceSpirit : MonsterAI
{
    private Transform closestTarget;
    private float projectileSpeed = 3; // 발사체 속도 설정
    private string projectilePrefab = "Projectile/Ice"; // 발사체 프리팹 참조

    // Start() 함수: 몬스터 초기 설정
    public override void Start()
    {
        MaxHp = 60f;
        base.Start(); // MonsterAI 스크립트의 Start() 함수 호출 (기본 AI 로직 유지)
        Speed = 3f; // 몬스터 이동 속도 설정
        AttackCooldown = 8.0f; // 공격 쿨타임 설정
        attackRange = 25.0f; // 공격 범위 설정
        MaxHp = 20f;
        CurHp = MaxHp;
    }

    // Update() 함수: 매 프레임마다 호출되는 함수
    public override void Update()
    {
        closestTarget = GetClosestTarget();

        float distanceToTarget = Vector3.Distance(transform.position, closestTarget.position); // 플레이어와의 거리 계산

        // 공격 범위 밖에 있으면 플레이어를 추적
        if (distanceToTarget > attackRange)
        {
            agent.SetDestination(closestTarget.position); // 플레이어 위치로 이동
        }
        else
        {
            agent.ResetPath(); // 공격 범위 안에 있을 때는 멈춤

            // 공격 쿨타임이 다 지나면 발사체 발사
            if (attackTimer <= 0f)
            {
                LaunchProjectile(); // 발사체 발사
                attackTimer = attackCooldown; // 쿨타임 리셋
            }
        }

        // 공격 타이머를 감소시킴 (쿨타임 처리)
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    private Transform GetClosestTarget()
    {
        float closestSqrDistance = Mathf.Infinity;
        Transform closestTarget = null;

        string[] tags = { "skilltower", "turret", "Castle", "Player" };

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

                if (tag == "player")
                {
                    PlayerController playerScript = target.GetComponent<PlayerController>();
                    if (playerScript != null && !playerScript.isDie)
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

    private void LaunchProjectile()
    {
        // 발사체를 PhotonNetwork.Instantiate를 사용하여 생성
        GameObject projectile = PhotonNetwork.Instantiate(projectilePrefab, transform.position, Quaternion.identity);

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
