using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceMagic : PlayerController
{
    float[] skillRanges = { 10f, 10f, 10f };

    public GameObject skillRangeA;
    private Vector3 skillPosA;
    private float skillCooltimeA;

    public GameObject frozenawlPrefab;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        if (pv.IsMine)
        {
            if (skillCooltimeA >= 0) { skillCooltimeA -= Time.deltaTime; }
            // if (skillCooltimeS >= 0) { skillCooltimeS -= Time.deltaTime; }
            // if (skillCooltimeD >= 0) { skillCooltimeD -= Time.deltaTime; }
        }
        if (!isCasting)
        {
            PlayerSkillA();
            // PlayerSkillS();
            // PlayerSkillD();
        }
    }

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);
        if (stream.IsWriting)
        {
            stream.SendNext(skillPosA);
            // stream.SendNext(skillPosS);
            // stream.SendNext(skillPosD);
        }
        else
        {
            skillPosA = (Vector3)stream.ReceiveNext();
            // skillPosS = (Vector3)stream.ReceiveNext();
            // skillPosD = (Vector3)stream.ReceiveNext();
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
                    pv.RPC("PlayAnimation", RpcTarget.All, "FrozenAwl");
                }
            }
        }
    }

    public void OnUseSkillA()
    {
        if (pv.IsMine)
        {
            pv.RPC("UseFrozenAwl", RpcTarget.All, null);
        }
    }

    [PunRPC]
    void UseFrozenAwl()
    {
        Quaternion fireRot = transform.rotation * Quaternion.Euler(new Vector3(0, 180, 0));
        GameObject fire = Instantiate(frozenawlPrefab, transform.position + Vector3.up / 2, fireRot);
        fire.GetComponent<FrozenAwl>().targetPos = skillPosA;
    }
}
