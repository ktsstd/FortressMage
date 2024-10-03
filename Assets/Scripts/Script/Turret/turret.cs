using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Button increaseFireRateButton;

    // Damage 관련 변수
    public float damage = 20f;
    public float increaseAmount = 5f;
    private Button increaseDamageButton;

    // 공통 변수
    public GoldManager goldManager; // 골드 매니저 참조

    // Turret 업그레이드 관련 변수
    public int turretUpgradeCost = 50; // 포탑 업그레이드 비용
    public int turretMaxUpgrades = 5; // 포탑 최대 업그레이드 횟수
    public float turretCostIncreaseRate = 1.5f; // 포탑 업그레이드 비용 증가율
    private int turretCurrentUpgrades = 0; // 포탑 현재 업그레이드 횟수

    // Damage 업그레이드 관련 변수
    public int damageUpgradeCost = 50; // 데미지 업그레이드 비용
    public int damageMaxUpgrades = 10; // 데미지 최대 업그레이드 횟수
    public float damageCostIncreaseRate = 1.2f; // 데미지 업그레이드 비용 증가율
    private int damageCurrentUpgrades = 0; // 데미지 현재 업그레이드 횟수

    // Radius 업그레이드 관련 변수
    public float explosionRadiusUpgradeAmount = 1f;  // 범위 업그레이드 크기
    public int explosionRadiusUpgradeCost = 50;  // 범위 업그레이드 비용
    public int explosionRadiusMaxUpgrades = 5;  // 범위 업그레이드 최대 횟수
    private int explosionRadiusCurrentUpgrades = 0;  // 범위 업그레이드 횟수

    private void Start()
    {
        // Turret 자동 공격 시작
        StartCoroutine(FireContinuously());

        // 버튼 클릭 시 각 함수 호출 연결
        if (increaseFireRateButton != null)
        {
            increaseFireRateButton.onClick.AddListener(IncreaseFireRate);
        }

        if (increaseDamageButton != null)
        {
            increaseDamageButton.onClick.AddListener(IncreaseDamage);
        }
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
    GameObject closestEliteTarget = null;
    GameObject closestRegularTarget = null;
    float closestEliteDistance = Mathf.Infinity;
    float closestRegularDistance = Mathf.Infinity;

    foreach (Collider target in targetsInViewRadius)
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        
        // 엘리트 적일 경우
        if (target.CompareTag("Elite"))
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

    // 엘리트 적이 있으면 우선 공격, 없으면 가까운 일반 적 공격
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

            // 거리에 따라 발사 속도 조절
            float launchPower = CalculateLaunchPowerByDistance(distance);

            Vector3 force = direction * launchPower;  // 거리 기반 발사 속도 적용
            rb.AddForce(force, ForceMode.Impulse);
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

    public void IncreaseFireRate()
    {
        // 포탑 업그레이드 제한 확인
        if (turretCurrentUpgrades < turretMaxUpgrades)
        {
            if (goldManager.SpendGold(turretUpgradeCost))
            {
                fireRate -= fireRateIncreaseAmount;

                if (fireRate < maxFireRate)
                {
                    fireRate = maxFireRate;
                }

                turretCurrentUpgrades++;  // 포탑 업그레이드 횟수 증가
                turretUpgradeCost = Mathf.RoundToInt(turretUpgradeCost * turretCostIncreaseRate); // 업그레이드 비용 증가
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    // Damage 관련 기능
    public void IncreaseDamage()
    {
        // 데미지 업그레이드 가능 여부 확인
        if (damageCurrentUpgrades < damageMaxUpgrades)
        {
            // 골드가 충분하면 업그레이드 실행
            if (goldManager.SpendGold(damageUpgradeCost))
            {
                damage += increaseAmount;  // 데미지 증가
                damageCurrentUpgrades++;  // 업그레이드 횟수 증가
                damageUpgradeCost = Mathf.RoundToInt(damageUpgradeCost * damageCostIncreaseRate);  // 업그레이드 비용 증가
            }
        }
    }

    // Radius 관련 기능
    public void UpgradeExplosionRadius()
    {
        if (explosionRadiusCurrentUpgrades < explosionRadiusMaxUpgrades)
        {
            // 골드가 충분한지 확인 후 업그레이드
            if (goldManager.SpendGold(explosionRadiusUpgradeCost))
            {
                explosionRadius += explosionRadiusUpgradeAmount;  // 폭발 범위 증가
                explosionRadiusCurrentUpgrades++;  // 업그레이드 횟수 증가
                explosionRadiusUpgradeCost = Mathf.RoundToInt(explosionRadiusUpgradeCost * 1.5f);  // 업그레이드 비용 증가
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