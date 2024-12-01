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

    public GameObject skillRangeS;
    private Vector3 skillPosS;
    private float skillCooltimeS;

    public GameObject skillRangeD;
    private Vector3 skillPosD;
    private float skillCooltimeD;

    public GameObject electricshotPrefab;
    public GameObject shockwavePrefab;
    public GameObject tempestfuryPrefab;

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
            if (skillCooltimeS >= 0) { skillCooltimeS -= Time.deltaTime; }
            if (skillCooltimeD >= 0) { skillCooltimeD -= Time.deltaTime; }
        }
        if (!isCasting && !isStun)
        {
            if (!skillRangeS.activeSelf && !skillRangeD.activeSelf)
                PlayerSkillA();
            if (!skillRangeA.activeSelf && !skillRangeD.activeSelf)
                PlayerSkillS();
            if (!skillRangeA.activeSelf && !skillRangeS.activeSelf)
                PlayerSkillD();
        }
    }

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);
        if (stream.IsWriting)
        {
            stream.SendNext(skillPosA);
            stream.SendNext(skillPosS);
            stream.SendNext(skillPosD);
        }
        else
        {
            skillPosA = (Vector3)stream.ReceiveNext();
            skillPosS = (Vector3)stream.ReceiveNext();
            skillPosD = (Vector3)stream.ReceiveNext();
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

    void PlayerSkillS()
    {
        if (pv.IsMine)
        {
            if (skillCooltimeS <= 0)
            {
                if (Input.GetKey(KeyCode.S))
                {
                    skillRangeS.SetActive(true);
                }
                if (Input.GetKeyUp(KeyCode.S))
                {
                    skillRangeS.SetActive(false);
                    skillPosS = skillRangeS.transform.position;
                    transform.rotation = Quaternion.LookRotation(GetSkillRange(skillRanges[1]) - transform.position);
                    skillCooltimeS = 5f;
                    pv.RPC("PlayAnimation", RpcTarget.All, "ShockWave");
                    pv.RPC("UseShockWave", RpcTarget.All, skillPosS);
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
                if (Input.GetKey(KeyCode.D))
                {
                    skillRangeD.SetActive(true);
                    skillRangeD.transform.position = new Vector3(GetSkillRange(skillRanges[2]).x, 0.1f, GetSkillRange(skillRanges[2]).z);
                }
                if (Input.GetKeyUp(KeyCode.D))
                {
                    skillRangeD.SetActive(false);
                    skillCooltimeD = 1f;
                    skillPosD = new Vector3(GetSkillRange(skillRanges[2]).x, 0.1f, GetSkillRange(skillRanges[2]).z);
                    transform.rotation = Quaternion.LookRotation(GetSkillRange(skillRanges[2]) - transform.position);
                    pv.RPC("PlayAnimation", RpcTarget.All, "TempestFury");
                    pv.RPC("UseTempestFury", RpcTarget.All, skillPosD);
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

    [PunRPC]
    void UseShockWave(Vector3 _skillPos)
    {
        GameObject fire = Instantiate(shockwavePrefab, _skillPos, transform.rotation);
    }

    [PunRPC]
    void UseTempestFury(Vector3 _skillPos)
    {
        GameObject fire = Instantiate(tempestfuryPrefab, _skillPos, transform.rotation);
    }
}
