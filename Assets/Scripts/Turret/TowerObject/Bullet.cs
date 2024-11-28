using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    public float damage;               // �߻�ü�� ���� ������
    public float explosionRadius = 3f; // ���� ���� �ݰ�
    public LayerMask enemyLayer;       // ���� ���� ���̾�

    // �߻�ü�� ������ �� �ʿ��� �������� ���� �ݰ��� �����ϴ� �Լ�
    public void Initialize(float bulletDamage, float bulletExplosionRadius)
    {
        damage = bulletDamage;  // Turret�̳� �ٸ� �ý��ۿ��� ���޵� ������ ��
        explosionRadius = bulletExplosionRadius;  // ���� �ݰ� ����
    }

     private void OnTriggerEnter(Collider collision)
    {
        // �� �Ǵ� �ٴڿ� �浹 �� ���� ó��
        if (collision.gameObject.CompareTag("Enemy") || 
            collision.gameObject.CompareTag("Ground"))
        {
            // ���� ����ȭ ���� (������ Ŭ���̾�Ʈ������)
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("Explode", RpcTarget.AllBuffered); // ���� ó�� ����ȭ
            }

            // �߻�ü ����
            PhotonNetwork.Destroy(gameObject);
        }
    }

[PunRPC]
    void Explode()
    {
        // ���� �ݰ� ���� �� Ž��
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);

        foreach (Collider enemyCollider in enemiesInRange)
        {
            MonsterAI monster = enemyCollider.GetComponent<MonsterAI>();
            if (monster != null)
            {
                // ���Ϳ��� ���� ������ ����
                monster.MonsterDmged((int)damage);
            }
        }
    }

    // ���� ������ �ð������� Ȯ���ϱ� ���� ����׿�
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}