using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectroMagic : PlayerController
{
    float[] skillRanges = { 10f, 10f, 10f };

    public GameObject skillRangeA;
    private Vector3 skillPosA;
    private float skillCooltimeA;

    public GameObject electricshotPrefab;

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
                    Vector3 direction = mousePosition - transform.position;
                    float distance = direction.magnitude;
                    direction = direction.normalized;

                    skillRangeA.transform.position = transform.position + direction * (skillRanges[0] * 0.5f);
                    skillRangeA.transform.rotation = Quaternion.LookRotation(GetSkillRange(skillRanges[0]) - transform.position) * Quaternion.Euler(new Vector3(0, -90, 0));
                }
                if (Input.GetKeyUp(KeyCode.A))
                {
                    skillRangeA.SetActive(false);
                    skillPosA = skillRangeA.transform.position;
                    transform.rotation = Quaternion.LookRotation(GetSkillRange(skillRanges[0]) - transform.position);
                    skillCooltimeA = 2f;
                    pv.RPC("PlayAnimation", RpcTarget.All, "ElectricShot");
                }
            }
        }
    }

    public void OnUseSkillA()
    {
        if (pv.IsMine)
        {
            pv.RPC("UseElectricShot", RpcTarget.All, null);
        }
    }

    [PunRPC]
    void UseElectricShot()
    {
        GameObject fire = Instantiate(electricshotPrefab, skillPosA, transform.rotation);
    }
}
