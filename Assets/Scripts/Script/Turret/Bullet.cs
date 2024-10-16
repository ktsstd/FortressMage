using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
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
        // �浹�� ������Ʈ�� "Enemy", "Elite" �Ǵ� "Ground" �±׸� ���� ������Ʈ���� Ȯ��
        if (collision.gameObject.CompareTag("Enemy") || 
            collision.gameObject.CompareTag("Elite") || 
            collision.gameObject.CompareTag("Ground"))
        {
            // ���� ���� ����
            Explode();

            // �߻�ü ����
            Destroy(gameObject);
        }
    }

    // ���� ������ �����ϴ� �Լ�
    void Explode()
    {
        // ���� ���� ���� �� Ž��
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);

        // ���� �� ��� ������ ���� ����
        foreach (Collider enemyCollider in enemiesInRange)
        {
            //Enemy enemy = enemyCollider.GetComponent<Enemy>();
            //if (enemy != null)
            //{
            //    enemy.TakeDamage(damage);  // ������ �������� ����
            //}
        }
    }

    // ���� ������ �ð������� Ȯ���ϱ� ���� ����׿�
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}