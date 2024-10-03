using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Button increaseFireRateButton;

    // Damage ���� ����
    public float damage = 20f;
    public float increaseAmount = 5f;
    private Button increaseDamageButton;

    // ���� ����
    public GoldManager goldManager; // ��� �Ŵ��� ����

    // Turret ���׷��̵� ���� ����
    public int turretUpgradeCost = 50; // ��ž ���׷��̵� ���
    public int turretMaxUpgrades = 5; // ��ž �ִ� ���׷��̵� Ƚ��
    public float turretCostIncreaseRate = 1.5f; // ��ž ���׷��̵� ��� ������
    private int turretCurrentUpgrades = 0; // ��ž ���� ���׷��̵� Ƚ��

    // Damage ���׷��̵� ���� ����
    public int damageUpgradeCost = 50; // ������ ���׷��̵� ���
    public int damageMaxUpgrades = 10; // ������ �ִ� ���׷��̵� Ƚ��
    public float damageCostIncreaseRate = 1.2f; // ������ ���׷��̵� ��� ������
    private int damageCurrentUpgrades = 0; // ������ ���� ���׷��̵� Ƚ��

    // Radius ���׷��̵� ���� ����
    public float explosionRadiusUpgradeAmount = 1f;  // ���� ���׷��̵� ũ��
    public int explosionRadiusUpgradeCost = 50;  // ���� ���׷��̵� ���
    public int explosionRadiusMaxUpgrades = 5;  // ���� ���׷��̵� �ִ� Ƚ��
    private int explosionRadiusCurrentUpgrades = 0;  // ���� ���׷��̵� Ƚ��

    private void Start()
    {
        // Turret �ڵ� ���� ����
        StartCoroutine(FireContinuously());

        // ��ư Ŭ�� �� �� �Լ� ȣ�� ����
        if (increaseFireRateButton != null)
        {
            increaseFireRateButton.onClick.AddListener(IncreaseFireRate);
        }

        if (increaseDamageButton != null)
        {
            increaseDamageButton.onClick.AddListener(IncreaseDamage);
        }
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
    GameObject closestEliteTarget = null;
    GameObject closestRegularTarget = null;
    float closestEliteDistance = Mathf.Infinity;
    float closestRegularDistance = Mathf.Infinity;

    foreach (Collider target in targetsInViewRadius)
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        
        // ����Ʈ ���� ���
        if (target.CompareTag("Elite"))
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

    // ����Ʈ ���� ������ �켱 ����, ������ ����� �Ϲ� �� ����
    if (closestEliteTarget != null)
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

    public void IncreaseFireRate()
    {
        // ��ž ���׷��̵� ���� Ȯ��
        if (turretCurrentUpgrades < turretMaxUpgrades)
        {
            if (goldManager.SpendGold(turretUpgradeCost))
            {
                fireRate -= fireRateIncreaseAmount;

                if (fireRate < maxFireRate)
                {
                    fireRate = maxFireRate;
                }

                turretCurrentUpgrades++;  // ��ž ���׷��̵� Ƚ�� ����
                turretUpgradeCost = Mathf.RoundToInt(turretUpgradeCost * turretCostIncreaseRate); // ���׷��̵� ��� ����
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    // Damage ���� ���
    public void IncreaseDamage()
    {
        // ������ ���׷��̵� ���� ���� Ȯ��
        if (damageCurrentUpgrades < damageMaxUpgrades)
        {
            // ��尡 ����ϸ� ���׷��̵� ����
            if (goldManager.SpendGold(damageUpgradeCost))
            {
                damage += increaseAmount;  // ������ ����
                damageCurrentUpgrades++;  // ���׷��̵� Ƚ�� ����
                damageUpgradeCost = Mathf.RoundToInt(damageUpgradeCost * damageCostIncreaseRate);  // ���׷��̵� ��� ����
            }
        }
    }

    // Radius ���� ���
    public void UpgradeExplosionRadius()
    {
        if (explosionRadiusCurrentUpgrades < explosionRadiusMaxUpgrades)
        {
            // ��尡 ������� Ȯ�� �� ���׷��̵�
            if (goldManager.SpendGold(explosionRadiusUpgradeCost))
            {
                explosionRadius += explosionRadiusUpgradeAmount;  // ���� ���� ����
                explosionRadiusCurrentUpgrades++;  // ���׷��̵� Ƚ�� ����
                explosionRadiusUpgradeCost = Mathf.RoundToInt(explosionRadiusUpgradeCost * 1.5f);  // ���׷��̵� ��� ����
            }
        }
    }

    public float GetDamage()
    {
        return damage;
    }

     public float GetExplosionRadius()
    {
        return explosionRadius;
    }
}