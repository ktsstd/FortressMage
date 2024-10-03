using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet2 : MonoBehaviour
{
    public float damage;               // �߻�ü�� ���� ������
    public float explosionRadius = 3f; // ���� ���� �ݰ�
    private Turret Turret2;             // �ͷ� ������ �� ���� ������ ���� ����
    public LayerMask enemyLayer;       // ���� ���� ���̾�

    private void Start()
    {
        // ���� ���� Turret�� ã�Ƽ� �������� ���� �ݰ��� �޾ƿ�
        Turret2 = FindObjectOfType<Turret>();

        if (Turret2 != null)
        {
            // Turret2�� �������� ���� �ݰ��� �߻�ü�� ����
            damage = Turret2.GetDamage();
            explosionRadius = Turret2.GetExplosionRadius();  // �ͷ����� ���� �ݰ� ������
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� "Ground" �±׸� ���� ������Ʈ���� Ȯ��
        if (collision.gameObject.CompareTag("Ground"))
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
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);  // ������ �������� ����
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
