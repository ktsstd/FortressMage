using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMagic : PlayerController, ISkillAnimationEvent
{
    float[] skillRanges = { 10f, 10f, 10f };

    public GameObject skillRangeA;
    private Vector3 skillPosA;
    private float skillCooltimeA;

    public GameObject skillRangeS;
    private Vector3 skillPosS;
    private float skillCooltimeS;

    public GameObject skillEffectD;
    private float skillCooltimeD;

    public GameObject fireballPrefab;
    public GameObject firestormPrefab;

    public AudioClip phoenix;


    public override void Start()
    {
        base.Start();
        elementalCode = 1;
        playerAtk = 15;
        defaultAtk = 15;
        skillMaxCooltimeA = 4f;
        skillMaxCooltimeS = 8f;
        skillMaxCooltimeD = 30f;
    }
    public override void Update()
    {
        base.Update();
        if (pv.IsMine)
        {
            if (!isDie)
            {
                if (skillCooltimeA >= 0) { skillCooltimeA -= Time.deltaTime; playerUi.skillsCoolTime[0] = skillCooltimeA; playerUi.skillsMaxCoolTime[0] = skillMaxCooltimeA; }
                if (skillCooltimeS >= 0) { skillCooltimeS -= Time.deltaTime; playerUi.skillsCoolTime[1] = skillCooltimeS; playerUi.skillsMaxCoolTime[1] = skillMaxCooltimeS; }
                if (skillCooltimeD >= 0) { skillCooltimeD -= Time.deltaTime; playerUi.skillsCoolTime[2] = skillCooltimeD; playerUi.skillsMaxCoolTime[2] = skillMaxCooltimeD; }

                if (!isCasting && !isStun)
                {
                    if (!skillRangeS.activeSelf)
                        PlayerSkillA();
                    if (!skillRangeA.activeSelf)
                        PlayerSkillS();
                    if (!skillRangeA.activeSelf && !skillRangeS.activeSelf)
                        PlayerSkillD();
                }
            }
        }
    }

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);
        if (stream.IsWriting)
        {
            stream.SendNext(skillPosA);
            stream.SendNext(skillPosS);
        }
        else
        {
            skillPosA = (Vector3)stream.ReceiveNext();
            skillPosS = (Vector3)stream.ReceiveNext();
        }
    }

    void PlayerSkillA()
    {
        if (pv.IsMine)
        {
            if (skillCooltimeA <= 0)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    skillRangeA.SetActive(true);
                    skillRangeA.transform.position = Vector3.Lerp(transform.position, GetSkillRange(skillRanges[0]), 0.5f);
                    skillRangeA.transform.localScale = new Vector3(1f, 0.1f, (GetSkillRange(skillRanges[0]) - transform.position).magnitude) * 2;
                    skillRangeA.transform.rotation = Quaternion.LookRotation(GetSkillRange(skillRanges[0]) - transform.position);
                }
                if (Input.GetKeyUp(KeyCode.A))
                {
                    skillRangeA.SetActive(false);
                    transform.rotation = Quaternion.LookRotation(GetSkillRange(skillRanges[0]) - transform.position);
                    skillPosA = new Vector3(GetSkillRange(skillRanges[0]).x, transform.position.y + 0.5f, GetSkillRange(skillRanges[0]).z);
                    skillCooltimeA = skillMaxCooltimeA;
                    pv.RPC("PlayAnimation", RpcTarget.All, "FireBall");
                }
            }
        }
    }

    void PlayerSkillS()
    {
        if (pv.IsMine)
        {
            if (skillCooltimeS <= 0)
            {
                if (Input.GetKey(KeyCode.S))
                {
                    skillRangeS.SetActive(true);
                    skillRangeS.transform.position = new Vector3(GetSkillRange(skillRanges[1]).x, 0.1f, GetSkillRange(skillRanges[1]).z);
                }
                if (Input.GetKeyUp(KeyCode.S))
                {
                    skillRangeS.SetActive(false);
                    skillPosS = new Vector3(GetSkillRange(skillRanges[1]).x, 0.1f, GetSkillRange(skillRanges[1]).z);
                    transform.rotation = Quaternion.LookRotation(GetSkillRange(skillRanges[0]) - transform.position);
                    skillCooltimeS = skillMaxCooltimeS;
                    pv.RPC("PlayAnimation", RpcTarget.All, "FireStorm");
                }
            }
        }
    }

    void PlayerSkillD()
    {
        if (pv.IsMine)
        {
            if (skillCooltimeD <= 0)
            {
                if (Input.GetKeyDown(KeyCode.D))
                {

                    pv.RPC("UsePhoenix", RpcTarget.All, null);
                    pv.RPC("PlayAnimation", RpcTarget.All, "Phoenix");
                    skillCooltimeD = skillMaxCooltimeD;
                    playerAtk += defaultAtk;
                }
            }
        }
    }

    void OffPhoenix()
    {
        playerAtk -= defaultAtk;
        skillEffectD.SetActive(false);
    }

    public void OnUseSkillA()
    {
        if (pv.IsMine)
        {
            pv.RPC("UseFireBall", RpcTarget.All, (int)playerAtk);
        }
    }
    public void OnUseSkillS()
    {
        if (pv.IsMine)
        {
            pv.RPC("UseFireStorm", RpcTarget.All, (int)(playerAtk / 5));
        }
    }

    [PunRPC]
    void UseFireBall(int _damage)
    {
        Quaternion fireRot = transform.rotation * Quaternion.Euler(new Vector3(0,180,0));
        GameObject fire = Instantiate(fireballPrefab, transform.position + Vector3.up / 2, fireRot);
        fire.GetComponent<FireBall>().targetPos = skillPosA;
        fire.GetComponent<FireBall>().damage = _damage;
    }

    [PunRPC]
    void UseFireStorm(int _damage)
    {
        GameObject fire = Instantiate(firestormPrefab, skillPosS, transform.rotation);
        fire.GetComponent<FireStorm>().damage = _damage;
    }

    [PunRPC]
    void UsePhoenix()
    {
        skillEffectD.SetActive(true);
        audioSource.PlayOneShot(phoenix, 0.05f);
        Invoke("OffPhoenix", 15f);
    }
}
