using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Turret : MonoBehaviourPun
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float minLaunchPower = 10f;
    public float maxLaunchPower = 50f;
    public float fireRate = 1f;
    public float maxFireRate = 0.2f;
    public float explosionRadius = 3f;
    public float fireRateIncreaseAmount = 0.2f;
    public LayerMask targetLayer;
    public float detectionRadius = 10f;
    public float health = 100f;

    public float damage = 20f;
    public float increaseAmount = 5f;
    
    private bool canAttack = true; // 공격 여부를 체크하는 변수

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient) // 호스트만 자동 공격 실행
        {
            StartCoroutine(FireContinuously());
        }
    }

    private void Update()
    {
        // 스페이스바 입력 감지
        if (Input.GetKeyDown(KeyCode.A))
        {
            TakeDamage(damage); // 데미지를 입힘
        }
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
        // 네트워크를 통해 발사체 생성
        GameObject projectile = PhotonNetwork.Instantiate(projectilePrefab.name, firePoint.position, Quaternion.identity);
        
        // 발사체에 PhotonView를 추가하여 물리 동작을 동기화
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

                // 물리적으로 발사체를 날리는 RPC 호출
                photonView.RPC("ApplyForceToProjectile", RpcTarget.AllBuffered, projectilePhotonView.ViewID, force);
            }

            Bullet bullet = projectile.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.Initialize(damage, explosionRadius);
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

     public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            health = 0f;
            canAttack = false;
            
            // GameManager의 Wave 값을 가져와서 비교
            float currentWave = GameManager.Instance.GetWave();
            GameManager.Instance.CheckTurretDestroyedOnWave(currentWave); // 현재 웨이브 전달
        }
    }

    public void ResetHealth()
    {
        health = 100f; // 체력을 100으로 리셋
        canAttack = true; // 공격 가능하게 설정
        StartCoroutine(FireContinuously()); // 공격 코루틴 다시 시작
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}