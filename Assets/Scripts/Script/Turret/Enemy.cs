using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100f;  // ���� ü��

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        // �� ��� ó��
        Destroy(gameObject);  // �� ������Ʈ�� ����
    }
}
