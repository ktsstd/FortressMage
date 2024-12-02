using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Skilltower : MonoBehaviourPun
{
    public float health = 100f;

    public GameObject Lazer;
    public Transform LazerPosition;
    public float cooldownTime = 2f;
    private float lastSkillTime = -Mathf.Infinity;

    public float destroyDelay = 4.5f;
    public bool canAttack = true; // ���� ���θ� üũ�ϴ� ����
    private Animator animator; // Animator 컴포넌트 참조
    public GameObject explosionEffectPrefab; // 폭발 이펙트 프리팹
    private bool hasExploded = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // Q키가 눌렸을 때만 레이저 발사
            if (Input.GetKeyDown(KeyCode.Q) && Time.time >= lastSkillTime + cooldownTime)
            {
                if (Lazer != null)
                {
                    // 레이저 오브젝트를 생성하고 쿨타임 업데이트
                    GameObject laserInstance = Instantiate(Lazer, LazerPosition.position, Quaternion.identity);
                    lastSkillTime = Time.time;

                    // 4초 후에 레이저 오브젝트 삭제
                    Destroy(laserInstance, destroyDelay);
                }
            }
        }
    }


    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            health = 0f;
            canAttack = false;

            photonView.RPC("HandleDestruction", RpcTarget.AllBuffered);

            // GameManager의 Wave 값을 가져와서 비교
            float currentWave = GameManager.Instance.GetWave();
            GameManager.Instance.CheckTurretDestroyedOnWave(currentWave); // 현재 웨이브 전달
        }
    }

    public void UseMeteor(Vector3 _skillPos)
    {
        
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
        };
    }

    private void CreateExplosionEffect()
    {
        if (explosionEffectPrefab != null)
        {
            // 폭발 이펙트 생성
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            // 1초 후 이펙트 제거
            Destroy(explosion, 1f);
        }
    }

    public void ResetHealth()
    {
        health = 100f; // ü���� 100���� ����
        canAttack = true; // ���� �����ϰ� ����
        hasExploded = false;
        if (animator != null)
        {
            animator.ResetTrigger("DestroyTrigger"); // "DestroyTrigger" 초기화
            animator.SetTrigger("RebuildTrigger");   // 재구축 애니메이션 트리거
        }
    }
}
