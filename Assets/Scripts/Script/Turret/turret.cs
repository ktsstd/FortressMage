using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    // Turret ���� ����
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float minLaunchPower = 10f;  // �ּ� �߻� �ӵ�
    public float maxLaunchPower = 50f;  // �ִ� �߻� �ӵ�
    public float fireRate = 1f;
    public float maxFireRate = 0.2f;
    public float explosionRadius = 3f;
    public float fireRateIncreaseAmount = 0.2f;
    public LayerMask targetLayer;
    public float detectionRadius = 10f;

    // Damage ���� ����
    public float damage = 20f;
    public float increaseAmount = 5f;

    private void Start()
    {
        // Turret �ڵ� ���� ����
        StartCoroutine(FireContinuously());
    }

    // Turret ���� ���
    private IEnumerator FireContinuously()
    {
        while (true)
        {
            GameObject target = FindClosestTarget();
            if (target != null)
            {
                Fire(target.transform);
            }
            yield return new WaitForSeconds(fireRate);
        }
    }

    private GameObject FindClosestTarget()
    {
        // ��� �� Ž��
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);
        GameObject closestBossTarget = null;
        GameObject closestEliteTarget = null;
        GameObject closestRegularTarget = null;
        float closestBossDistance = Mathf.Infinity;
        float closestEliteDistance = Mathf.Infinity;
        float closestRegularDistance = Mathf.Infinity;

        foreach (Collider target in targetsInViewRadius)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            // ������ ���
            if (target.CompareTag("Boss"))
            {
                if (distanceToTarget < closestBossDistance)
                {
                    closestBossDistance = distanceToTarget;
                    closestBossTarget = target.gameObject;
                }
            }
            // ����Ʈ ���� ���
            else if (target.CompareTag("Elite"))
            {
                if (distanceToTarget < closestEliteDistance)
                {
                    closestEliteDistance = distanceToTarget;
                    closestEliteTarget = target.gameObject;
                }
            }
            // �Ϲ� ���� ���
            else 
            {
                if (distanceToTarget < closestRegularDistance)
                {
                    closestRegularDistance = distanceToTarget;
                    closestRegularTarget = target.gameObject;
                }
            }
        }

        // ������ ������ �켱 ����, ������ ����Ʈ ����, ������ ����� �Ϲ� �� ����
        if (closestBossTarget != null)
        {
            return closestBossTarget;
        }
        else if (closestEliteTarget != null)
        {
            return closestEliteTarget;
        }
        else
        {
            return closestRegularTarget;
        }   
    }   

    public void Fire(Transform target)
{
    GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
    Rigidbody rb = projectile.GetComponent<Rigidbody>();

    if (rb != null)
    {
        Vector3 direction = (target.position - firePoint.position).normalized;
        float distance = Vector3.Distance(firePoint.position, target.position);

        // �Ÿ��� ���� �߻� �ӵ� ����
        float launchPower = CalculateLaunchPowerByDistance(distance);
        Vector3 force = direction * launchPower;  // �Ÿ� ��� �߻� �ӵ� ����
        rb.AddForce(force, ForceMode.Impulse);
    }

    // �߻�ü�� �������� ���� ���� ����
    Bullet bullet = projectile.GetComponent<Bullet>();
    if (bullet != null)
    {
        bullet.Initialize(damage, explosionRadius);  // Turret�� �������� ���� �� ����
    }
}

    // �Ÿ��� ���� �߻� �ӵ� ��� �Լ� �߰�
    private float CalculateLaunchPowerByDistance(float distance)
    {
        // �Ÿ��� 0���� 1 ���� ������ ����ȭ (detectionRadius�� ����)
        float normalizedDistance = Mathf.Clamp01(distance / detectionRadius);

        // �ּ� �ӵ��� �ִ� �ӵ� ���̿��� ���� ����
        float launchPower = Mathf.Lerp(minLaunchPower, maxLaunchPower, normalizedDistance);
        return launchPower;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}