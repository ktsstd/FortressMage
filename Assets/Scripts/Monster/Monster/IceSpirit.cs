using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class IceSpirit : MonsterAI
{
    public float projectileSpeed = 3; // 발사체 속도 설정
    public GameObject projectilePrefab; // 발사체 프리팹 참조

    // Start() 함수: 몬스터 초기 설정
    public override void Start()
    {
        MaxHp = 60f;
        base.Start(); // MonsterAI 스크립트의 Start() 함수 호출 (기본 AI 로직 유지)
        Speed = 3f; // 몬스터 이동 속도 설정
        AttackCooldown = 8.0f; // 공격 쿨타임 설정
        attackRange = 25.0f; // 공격 범위 설정
        MaxHp = 40f;
        CurHp = MaxHp;
    }

    // Update() 함수: 매 프레임마다 호출되는 함수
    public override void Update()
    {
        if (player == null) return; // 플레이어가 없으면 동작하지 않음

        float distanceToPlayer = Vector3.Distance(transform.position, player.position); // 플레이어와의 거리 계산

        // 공격 범위 밖에 있으면 플레이어를 추적
        if (distanceToPlayer > attackRange)
        {
            agent.SetDestination(player.position); // 플레이어 위치로 이동
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

    // 발사체를 발사하는 함수
    private void LaunchProjectile()
    {
        // 발사체 생성 (현재 몬스터의 위치에서 생성)
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // 생성된 발사체에서 Projectile 스크립트를 가져옴
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        // 발사체 스크립트가 있으면 초기화 진행
        if (projectileScript != null)
        {
            // 플레이어 위치, 발사체 속도, 몬스터 데미지를 설정하여 발사체 초기화
            projectileScript.Initialize(player.position, projectileSpeed, MonsterDmg);
        }
    }
}
