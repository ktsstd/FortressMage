using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.ComponentModel;

public class Skilltower : MonoBehaviourPun, IPunObservable
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

    public PlayerUi playerUi;
    public PhotonView pv;

    public int[] elementalSet;
    public int[] receiveElemental;
    public Sprite[] elementals;

    public float[] elementalSetCoolTime;
    private float[] receiveElementalSetCoolTime;
    public float[] elementalSetMaxCoolTime;

    private void Start()
    {
        animator = GetComponent<Animator>();
        pv = GetComponent<PhotonView>();
        playerUi = FindObjectOfType<PlayerUi>();
    }

    private void Update()
    {
        for (int i = 0; i < 2; i++)
        {
            if (elementalSetCoolTime[i] >= 0)
            {
                elementalSetCoolTime[i] -= Time.deltaTime;
                playerUi.elementalSetCoolTime[i] = elementalSetCoolTime[i];
                playerUi.elementalSetMaxCoolTime[i] = elementalSetMaxCoolTime[i];
            }
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            elementalSet = receiveElemental;
            elementalSetCoolTime = receiveElementalSetCoolTime;
        }
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                stream.SendNext(elementalSet);
                stream.SendNext(elementalSetCoolTime);
            }
        }
        else
        {
            receiveElemental = (int[])stream.ReceiveNext();
            receiveElementalSetCoolTime = (float[])stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void SetingElemental(int _slot, int _set)
    {
        if (elementalSetCoolTime[_slot] <= 0)
        {
            elementalSet[_slot] = _set;
            playerUi.elementalSet[_slot].sprite = elementals[_set];
            elementalSetCoolTime[_slot] = elementalSetMaxCoolTime[_slot];
            elementalSetCoolTime[_slot] = elementalSetMaxCoolTime[_slot];
        }
    }

    [PunRPC]
    public void SettingMasterElemental()
    {

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
