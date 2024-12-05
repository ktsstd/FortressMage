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

    public float destroyDelay = 4.5f;
    public bool canAttack = true;
    private Animator animator; // Animator 컴포넌트 참조
    public GameObject explosionEffectPrefab; // 폭발 이펙트 프리팹
    private bool hasExploded = false;

    public PlayerUi playerUi;
    public PhotonView pv;

    public int[] elementalSet;
    public int[] receiveElemental;
    public Sprite[] elementals;
    public Sprite[] mixSkills;

    public float[] elementalSetCoolTime;
    public float[] elementalSetMaxCoolTime;

    //private float resetTime = 0.5f;

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

        if (pv.IsMine)
        {
            //if (resetTime >= 0) { resetTime -= Time.deltaTime; }
            //if (resetTime <= 0) { elementalSet = elementalSet; resetTime = 0.5f; }
        }
        else
        {
            elementalSet = receiveElemental;
        }
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(elementalSet);
        }
        else
        {
            receiveElemental = (int[])stream.ReceiveNext();
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

            if ((elementalSet[0] == 1 && elementalSet[1] == 2) || (elementalSet[0] == 2 && elementalSet[1] == 1))
                playerUi.mixSkill.sprite = mixSkills[0];
            else if ((elementalSet[0] == 1 && elementalSet[1] == 3) || (elementalSet[0] == 3 && elementalSet[1] == 1))
                playerUi.mixSkill.sprite = mixSkills[1];
            else if ((elementalSet[0] == 1 && elementalSet[1] == 4) || (elementalSet[0] == 4 && elementalSet[1] == 1))
                playerUi.mixSkill.sprite = mixSkills[2];
            else if ((elementalSet[0] == 2 && elementalSet[1] == 3) || (elementalSet[0] == 3 && elementalSet[1] == 2))
                playerUi.mixSkill.sprite = mixSkills[3];
            else if ((elementalSet[0] == 2 && elementalSet[1] == 4) || (elementalSet[0] == 4 && elementalSet[1] == 2))
                playerUi.mixSkill.sprite = mixSkills[4];
            else if ((elementalSet[0] == 3 && elementalSet[1] == 4) || (elementalSet[0] == 4 && elementalSet[1] == 3))
                playerUi.mixSkill.sprite = mixSkills[5];
            else
                playerUi.mixSkill.sprite = mixSkills[6];
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
