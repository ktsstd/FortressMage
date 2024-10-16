using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;               // 발사체가 입힐 데미지
    public float explosionRadius = 3f; // 범위 공격 반경
    public LayerMask enemyLayer;       // 적이 속한 레이어

    // 발사체가 생성될 때 필요한 데미지와 폭발 반경을 설정하는 함수
    public void Initialize(float bulletDamage, float bulletExplosionRadius)
    {
        damage = bulletDamage;  // Turret이나 다른 시스템에서 전달된 데미지 값
        explosionRadius = bulletExplosionRadius;  // 폭발 반경 설정
    }

    private void OnTriggerEnter(Collider collision)
    {
        // 충돌한 오브젝트가 "Enemy", "Elite" 또는 "Ground" 태그를 가진 오브젝트인지 확인
        if (collision.gameObject.CompareTag("Enemy") || 
            collision.gameObject.CompareTag("Elite") || 
            collision.gameObject.CompareTag("Ground"))
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
            //Enemy enemy = enemyCollider.GetComponent<Enemy>();
            //if (enemy != null)
            //{
            //    enemy.TakeDamage(damage);  // 적에게 데미지를 입힘
            //}
        }
    }

    // 폭발 범위를 시각적으로 확인하기 위한 디버그용
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}