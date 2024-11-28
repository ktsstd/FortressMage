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
    
    private bool canAttack = true; // ���� ���θ� üũ�ϴ� ����

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient) // ȣ��Ʈ�� �ڵ� ���� ����
        {
            StartCoroutine(FireContinuously());
        }
    }

    private void Update()
    {
        // �����̽��� �Է� ����
        if (Input.GetKeyDown(KeyCode.W))
        {
            TakeDamage(damage); // �������� ����
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
            
            // GameManager�� Wave ���� �����ͼ� ��
            float currentWave = GameManager.Instance.GetWave();
            GameManager.Instance.CheckTurretDestroyedOnWave(currentWave); // ���� ���̺� ����
        }
    }

    public void ResetHealth()
    {
        health = 100f; // ü���� 100���� ����
        canAttack = true; // ���� �����ϰ� ����
        StartCoroutine(FireContinuously()); // ���� �ڷ�ƾ �ٽ� ����
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}