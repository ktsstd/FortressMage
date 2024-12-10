using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    public float damage;               // �߻�ü�� ���� ������
    public float explosionRadius = 3f; // ���� ���� �ݰ�
    public LayerMask enemyLayer;       // ���� ���� ���̾�
    public GameObject explosionEffectPrefab; // ���� ����Ʈ ������
    public Transform target;

    // �߻�ü�� ������ �� �ʿ��� �������� ���� �ݰ��� �����ϴ� �Լ�
    public void Initialize(float bulletDamage, float bulletExplosionRadius, Transform targetTransform)
    {
        damage = bulletDamage;  // Turret�̳� �ٸ� �ý��ۿ��� ���޵� ������ ��
        explosionRadius = bulletExplosionRadius;  // ���� �ݰ� ����
        target = targetTransform;
    }

    void Update()
    {
        if (target != null)
        {
            // ��ǥ���� �������� ȸ��
            RotateTowardsTarget();
        }
    }

    private void RotateTowardsTarget()
    {
        // ��ǥ���� ���ϴ� ���� ���
        Vector3 direction = target.position - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2f);
        }
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
        if (PhotonNetwork.IsMasterClient)
        {
            CreateExplosionEffect();
        }

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

    void CreateExplosionEffect()
    {
        if (explosionEffectPrefab != null)
        {
            // ����Ʈ�� ���÷� ����
            GameObject effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

            float scale = explosionRadius * 2; // ������ -> �������� ��ȯ
            effect.transform.localScale = new Vector3(scale, scale, scale);

            Destroy(effect, 1.0f);
        }
    }

    // ���� ������ �ð������� Ȯ���ϱ� ���� ����׿�
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}