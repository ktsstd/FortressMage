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


    public override void Start()
    {
        base.Start();
    }
    public override void Update()
    {
        base.Update();
        if (pv.IsMine)
        {
            if (!isDie)
            {
                if (skillCooltimeA >= 0) { skillCooltimeA -= Time.deltaTime; }
                if (skillCooltimeS >= 0) { skillCooltimeS -= Time.deltaTime; }
                if (skillCooltimeD >= 0) { skillCooltimeD -= Time.deltaTime; }

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
                    skillCooltimeA = 3f;
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
                    skillCooltimeS = 5f;
                    pv.RPC("PlayAnimation", RpcTarget.All, "FireStorm");
                }
            }
        }
    }

    void PlayerSkillD() // 지금은 이펙트만 실행시키고 공격력 동기화는 레벨 만들때 처리 동기화처리는 OnPhotonSerializeView에서 하면 될듯
    {
        if (pv.IsMine)
        {
            if (skillCooltimeD <= 0)
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    pv.RPC("UsePhoenix", RpcTarget.All, null);
                    pv.RPC("PlayAnimation", RpcTarget.All, "Phoenix");
                }
            }
        }
    }

    void OffPhoenix()
    {
        // 공격력 정상화시키기
        skillEffectD.SetActive(false);
    }

    public void OnUseSkillA()
    {
        if (pv.IsMine)
        {
            pv.RPC("UseFireBall", RpcTarget.All, null);
        }
    }
    public void OnUseSkillS()
    {
        if (pv.IsMine)
        {
            pv.RPC("UseFireStorm", RpcTarget.All, null);
        }
    }

    [PunRPC]
    void UseFireBall()
    {
        Quaternion fireRot = transform.rotation * Quaternion.Euler(new Vector3(0,180,0));
        GameObject fire = Instantiate(fireballPrefab, transform.position + Vector3.up / 2, fireRot);
        fire.GetComponent<FireBall>().targetPos = skillPosA;
    }

    [PunRPC]
    void UseFireStorm()
    {
        GameObject fire = Instantiate(firestormPrefab, skillPosS, transform.rotation);
    }

    [PunRPC]
    void UsePhoenix()
    {
        skillCooltimeD = 30f;
        skillEffectD.SetActive(true);
        // 공격력 증가 처리하기
        Invoke("OffPhoenix", 15f);
    }
}
