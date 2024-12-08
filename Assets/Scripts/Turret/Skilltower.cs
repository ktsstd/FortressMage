using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.ComponentModel;

public class Skilltower : MonoBehaviourPun
{
    public float health = 100f;

    public float cooldownTime = 0f;
    public float maxCooldownTime = 20f;

    public float destroyDelay = 4.5f;
    public bool canAttack = true;
    private Animator animator; // Animator 컴포넌트 참조
    public GameObject explosionEffectPrefab; // 폭발 이펙트 프리팹
    private bool hasExploded = false;

    public PlayerUi playerUi;
    public PhotonView pv;

    public bool canUseSkill = false;
    public GameObject lazerPrefab;
    public GameObject meteorPrefab;
    public GameObject barricadePrefab;

    public GameObject laserRange;
    public GameObject meteorRange;
    public GameObject barricadeRange;

    public int[] elementalSet;
    public int mixSkillNum = 0;
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
            if (elementalSet[i] != 0)
            {
                if (elementalSetCoolTime[i] >= 0)
                {
                    elementalSetCoolTime[i] -= Time.deltaTime;
                    playerUi.elementalSetCoolTime[i] = elementalSetCoolTime[i];
                    playerUi.elementalSetMaxCoolTime[i] = elementalSetMaxCoolTime[i];
                }
            }
        }
        if (cooldownTime >= 0) { cooldownTime -= Time.deltaTime; }

        playerUi.mixCooldown = cooldownTime;
        playerUi.mixMaxCooldown = maxCooldownTime;


        if ((elementalSet[0] == 1 && elementalSet[1] == 2) || (elementalSet[0] == 2 && elementalSet[1] == 1))
            mixSkillNum = 1;
        else if ((elementalSet[0] == 1 && elementalSet[1] == 3) || (elementalSet[0] == 3 && elementalSet[1] == 1))
            mixSkillNum = 2;
        else if ((elementalSet[0] == 1 && elementalSet[1] == 4) || (elementalSet[0] == 4 && elementalSet[1] == 1))
            mixSkillNum = 3;
        else if ((elementalSet[0] == 2 && elementalSet[1] == 3) || (elementalSet[0] == 3 && elementalSet[1] == 2))
            mixSkillNum = 4;
        else if ((elementalSet[0] == 2 && elementalSet[1] == 4) || (elementalSet[0] == 4 && elementalSet[1] == 2))
            mixSkillNum = 5;
        else if ((elementalSet[0] == 3 && elementalSet[1] == 4) || (elementalSet[0] == 4 && elementalSet[1] == 3))
            mixSkillNum = 6;
        else
            mixSkillNum = 0;

        if (elementalSet[0] != 0 && elementalSet[1] != 0)
        {
            if (elementalSetCoolTime[0] <= 0 && elementalSetCoolTime[1] <= 0)
                canUseSkill = true;
            else
                canUseSkill = false;
        }  
        else
        {
            canUseSkill = false;
        }

        if (!canUseSkill)
        {
            laserRange.SetActive(false);
            meteorRange.SetActive(false);
            barricadeRange.SetActive(false);
        }
    }

    [PunRPC]
    public void SetingElemental(int _slot, int _set)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            elementalSet[_slot] = _set;

            playerUi.elementalSet[_slot].sprite = elementals[_set];
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

            pv.RPC("UpdateElemental", RpcTarget.Others, _slot, elementalSet[_slot]);
        }
    }

    [PunRPC]
    public void UpdateElemental(int _slot, int _set)
    {
        elementalSet[_slot] = _set;

        playerUi.elementalSet[_slot].sprite = elementals[_set];
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

    [PunRPC]
    public void UseMasterLazer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Instantiate(lazerPrefab);
            cooldownTime = maxCooldownTime;
            pv.RPC("UseLazer", RpcTarget.Others, null);
        }
    }
    [PunRPC]
    public void UseLazer()
    {
        Instantiate(lazerPrefab);
        cooldownTime = maxCooldownTime;
    }

    [PunRPC]
    public void UseMasterMeteor(Vector3 _skillPos)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Instantiate(meteorPrefab, _skillPos, Quaternion.Euler(0, 0, 0));
            cooldownTime = maxCooldownTime;
            pv.RPC("UseMeteor", RpcTarget.Others, _skillPos);
        }
    }
    [PunRPC]
    public void UseMeteor(Vector3 _skillPos)
    {
        Instantiate(meteorPrefab, _skillPos, Quaternion.Euler(0, 0, 0));
        cooldownTime = maxCooldownTime;
    }

    [PunRPC]
    public void UseMasterBarricade(Vector3 _skillPos)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Instantiate(barricadePrefab, _skillPos, Quaternion.Euler(0, 0, 0));
            cooldownTime = maxCooldownTime;
            pv.RPC("UseBarricade", RpcTarget.Others, _skillPos);
        }
    }
    [PunRPC]
    public void UseBarricade(Vector3 _skillPos)
    {
        Instantiate(barricadePrefab, _skillPos, Quaternion.Euler(0, 0, 0));
        cooldownTime = maxCooldownTime;
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
