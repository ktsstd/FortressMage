using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
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
        // 적 또는 바닥에 충돌 시 폭발 처리
        if (collision.gameObject.CompareTag("Enemy") || 
            collision.gameObject.CompareTag("Ground"))
        {
            // 폭발 동기화 실행 (마스터 클라이언트에서만)
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("Explode", RpcTarget.AllBuffered); // 폭발 처리 동기화
            }

            // 발사체 제거
            PhotonNetwork.Destroy(gameObject);
        }
    }

[PunRPC]
    void Explode()
    {
        // 폭발 반경 내의 적 탐색
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);

        foreach (Collider enemyCollider in enemiesInRange)
        {
            MonsterAI monster = enemyCollider.GetComponent<MonsterAI>();
            if (monster != null)
            {
                // 몬스터에게 폭발 데미지 적용
                monster.MonsterDmged((int)damage);
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