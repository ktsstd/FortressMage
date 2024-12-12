using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class Turret : MonoBehaviourPun
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public LayerMask targetLayer;

    public float minLaunchPower;
    public float maxLaunchPower;
    public float fireRate;
    public float maxFireRate;
    public float explosionRadius;
    public float fireRateIncreaseAmount;
    public float detectionRadius;
    public float health;
    public float maxHealth;

    public float damage;
    public float increaseAmount;

    public bool canAttack = true; // ���� ���θ� üũ�ϴ� ����
    private Animator animator; // Animator 컴포넌트 참조
    public GameObject explosionEffectPrefab; // 폭발 이펙트 프리팹

    private bool hasExploded = false;

    public Image barImage;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient) // ȣ��Ʈ�� �ڵ� ���� ����
        {
            StartCoroutine(FireContinuously());
        }

        maxHealth = 100f;
        health = maxHealth;

        minLaunchPower = 10f;
        maxLaunchPower = 50f;
        fireRate = 1f;
        maxFireRate = 0.2f;
        explosionRadius = 3f;
        fireRateIncreaseAmount = 0.2f;
        detectionRadius = 20f;

        damage = 2f;
        increaseAmount = 5f;

        canAttack = true;
        animator = GetComponent<Animator>();
    }
    private IEnumerator FireContinuously()
    {
        while (canAttack)
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
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider target in targetsInViewRadius)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToTarget < closestDistance)
            {
                closestDistance = distanceToTarget;
                closestTarget = target.gameObject;
            }
        }
        return closestTarget;
    }

    [PunRPC]
    public void Fire(Transform target)
    {
        // ��Ʈ��ũ�� ���� �߻�ü ����
        GameObject projectile = PhotonNetwork.Instantiate(projectilePrefab.name, firePoint.position, Quaternion.identity);

        // �߻�ü�� PhotonView�� �߰��Ͽ� ���� ������ ����ȭ
        PhotonView projectilePhotonView = projectile.GetComponent<PhotonView>();

        if (projectilePhotonView != null)
        {
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (target.position - firePoint.position).normalized;
                float distance = Vector3.Distance(firePoint.position, target.position);

                float launchPower = CalculateLaunchPowerByDistance(distance);
                Vector3 force = direction * launchPower;

                // ���������� �߻�ü�� ������ RPC ȣ��
                photonView.RPC("ApplyForceToProjectile", RpcTarget.AllBuffered, projectilePhotonView.ViewID, force);
            }

            Bullet bullet = projectile.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.Initialize(damage, explosionRadius, target);
            }
        }
    }

    [PunRPC]
    void ApplyForceToProjectile(int projectileViewID, Vector3 force)
    {
        PhotonView projectilePhotonView = PhotonView.Find(projectileViewID);
        if (projectilePhotonView != null)
        {
            Rigidbody rb = projectilePhotonView.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(force, ForceMode.Impulse);
            }
        }
    }

    private float CalculateLaunchPowerByDistance(float distance)
    {
        float normalizedDistance = Mathf.Clamp01(distance / detectionRadius);
        return Mathf.Lerp(minLaunchPower, maxLaunchPower, normalizedDistance);
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            health = 0f;
            canAttack = false;

            photonView.RPC("HandleDestruction", RpcTarget.AllBuffered);

            // GameManager�� Wave ���� �����ͼ� ��
            float currentWave = GameManager.Instance.GetWave();
            GameManager.Instance.CheckTurretDestroyedOnWave(currentWave); // ���� ���̺� ����
        }

        if (barImage != null)
        {
            float healthPercentage = health / maxHealth;
            photonView.RPC("UpdateHealthBar", RpcTarget.AllBuffered, healthPercentage);
        }
    }

    [PunRPC]
    private void HandleDestruction()
    {
        // 애니메이션 실행
        if (animator != null)
        {
            animator.SetTrigger("DestroyTrigger");
        }

        if (!hasExploded)
        {
            CreateExplosionEffect();
            hasExploded = true;  // 폭발 이펙트를 실행했음을 기록
        }
    }

    private void CreateExplosionEffect()
    {
        if (explosionEffectPrefab != null)
        {
            // 폭발 이펙트 생성
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

            // 폭발 반경에 맞게 크기 조정
            float scale = explosionRadius * 1;
            explosion.transform.localScale = new Vector3(scale, scale, scale);

            // 1초 후 이펙트 제거
            Destroy(explosion, 1f);
        }
    }

    [PunRPC]
    private void UpdateHealthBar(float healthPercentage)
    {
        barImage.fillAmount = healthPercentage;
    }


    [PunRPC]
    public void ResetHealth()
    {
        health = maxHealth; // ü���� 100���� ����
        canAttack = true; // ���� �����ϰ� ����
        hasExploded = false;
        if (animator != null)
        {
            animator.ResetTrigger("DestroyTrigger"); // "DestroyTrigger" 초기화
            animator.SetTrigger("RebuildTrigger");   // 재구축 애니메이션 트리거
        }
        StartCoroutine(FireContinuously()); // ���� �ڷ�ƾ �ٽ� ����

        if (barImage != null)
        {
            photonView.RPC("UpdateHealthBar", RpcTarget.AllBuffered, 1f); // 체력바를 처음에 꽉 차게 설정
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}