using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;               // 발사체가 입힐 데미지
    public float explosionRadius = 3f; // 범위 공격 반경
    private Turret turret;             // 터렛 데미지 및 범위 관리를 위한 참조
    public LayerMask enemyLayer;       // 적이 속한 레이어

    private void Start()
    {
        // 게임 내의 Turret를 찾아서 데미지와 폭발 반경을 받아옴
        turret = FindObjectOfType<Turret>();

        if (turret != null)
        {
            // Turret의 데미지와 범위 반경을 발사체로 설정
            damage = turret.GetDamage();
            explosionRadius = turret.GetExplosionRadius();  // 터렛에서 범위 반경 가져옴
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트가 "Ground" 태그를 가진 오브젝트인지 확인
        if (collision.gameObject.CompareTag("Ground"))
        {
            // 범위 공격 실행
            Explode();

            // 발사체 제거
            Destroy(gameObject);
        }
    }

    // 범위 공격을 수행하는 함수
    void Explode()
    {
        // 폭발 범위 내의 적 탐색
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);

        // 범위 내 모든 적에게 피해 적용
        foreach (Collider enemyCollider in enemiesInRange)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);  // 적에게 데미지를 입힘
            }
        }
    }

    // 폭발 범위를 시각적으로 확인하기 위한 디버그용
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
