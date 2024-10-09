using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    // Turret 관련 변수
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float minLaunchPower = 10f;  // 최소 발사 속도
    public float maxLaunchPower = 50f;  // 최대 발사 속도
    public float fireRate = 1f;
    public float maxFireRate = 0.2f;
    public float explosionRadius = 3f;
    public float fireRateIncreaseAmount = 0.2f;
    public LayerMask targetLayer;
    public float detectionRadius = 10f;

    // Damage 관련 변수
    public float damage = 20f;
    public float increaseAmount = 5f;

    private void Start()
    {
        // Turret 자동 공격 시작
        StartCoroutine(FireContinuously());
    }

    // Turret 관련 기능
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
        // 모든 적 탐색
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

            // 보스일 경우
            if (target.CompareTag("Boss"))
            {
                if (distanceToTarget < closestBossDistance)
                {
                    closestBossDistance = distanceToTarget;
                    closestBossTarget = target.gameObject;
                }
            }
            // 엘리트 적일 경우
            else if (target.CompareTag("Elite"))
            {
                if (distanceToTarget < closestEliteDistance)
                {
                    closestEliteDistance = distanceToTarget;
                    closestEliteTarget = target.gameObject;
                }
            }
            // 일반 적일 경우
            else 
            {
                if (distanceToTarget < closestRegularDistance)
                {
                    closestRegularDistance = distanceToTarget;
                    closestRegularTarget = target.gameObject;
                }
            }
        }

        // 보스가 있으면 우선 공격, 없으면 엘리트 공격, 없으면 가까운 일반 적 공격
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

        // 거리에 따라 발사 속도 조절
        float launchPower = CalculateLaunchPowerByDistance(distance);
        Vector3 force = direction * launchPower;  // 거리 기반 발사 속도 적용
        rb.AddForce(force, ForceMode.Impulse);
    }

    // 발사체에 데미지와 범위 값을 전달
    Bullet bullet = projectile.GetComponent<Bullet>();
    if (bullet != null)
    {
        bullet.Initialize(damage, explosionRadius);  // Turret의 데미지와 범위 값 전달
    }
}

    // 거리에 따른 발사 속도 계산 함수 추가
    private float CalculateLaunchPowerByDistance(float distance)
    {
        // 거리를 0에서 1 사이 값으로 정규화 (detectionRadius에 따라)
        float normalizedDistance = Mathf.Clamp01(distance / detectionRadius);

        // 최소 속도와 최대 속도 사이에서 선형 보간
        float launchPower = Mathf.Lerp(minLaunchPower, maxLaunchPower, normalizedDistance);
        return launchPower;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}