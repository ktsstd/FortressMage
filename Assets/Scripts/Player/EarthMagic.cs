using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthMagic : PlayerController
{

    float[] skillRanges = { 10f, 10f, 10f };

    public GameObject skillRangeA;
    private Vector3 skillPosA;
    private float skillCooltimeA;
    private float skillMaxCooltimeA = 3f;

    public GameObject skillRangeS;
    private Vector3 skillPosS;
    private float skillCooltimeS;
    private float skillMaxCooltimeS = 5f;

    public GameObject skillRangeD;
    private Vector3 skillPosD;
    private float skillCooltimeD;
    private float skillMaxCooltimeD = 20f;


    public GameObject stonePrefab;
    public GameObject earthquakePrefab;
    public GameObject stonewallPrefab;

    public override void Start()
    {
        base.Start();
        elementalCode = 4;
    }

    public override void Update()
    {
        base.Update();
        if (pv.IsMine)
        {
            if (skillCooltimeA >= 0) { skillCooltimeA -= Time.deltaTime; playerUi.skillsCoolTime[0] = skillCooltimeA; playerUi.skillsMaxCoolTime[0] = skillMaxCooltimeA; }
            if (skillCooltimeS >= 0) { skillCooltimeS -= Time.deltaTime; playerUi.skillsCoolTime[1] = skillCooltimeS; playerUi.skillsMaxCoolTime[1] = skillMaxCooltimeS; }
            if (skillCooltimeD >= 0) { skillCooltimeD -= Time.deltaTime; playerUi.skillsCoolTime[2] = skillCooltimeD; playerUi.skillsMaxCoolTime[2] = skillMaxCooltimeD; }

            if (!isDie)
            {
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
                    pv.RPC("PlayAnimation", RpcTarget.All, "StoneShoot");
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
                    skillRangeS.transform.position = Vector3.Lerp(transform.position, GetSkillRange(skillRanges[1]), 0.5f);
                    skillRangeS.transform.rotation = Quaternion.LookRotation(GetSkillRange(skillRanges[1]) - transform.position);
                }
                if (Input.GetKeyUp(KeyCode.S))
                {
                    skillRangeS.SetActive(false);
                    skillPosS = Vector3.Lerp(transform.position, GetSkillRange(skillRanges[1]), 0.5f);
                    transform.rotation = Quaternion.LookRotation(GetSkillRange(skillRanges[1]) - transform.position);
                    skillCooltimeS = skillMaxCooltimeS;
                    pv.RPC("PlayAnimation", RpcTarget.All, "Earthquake");
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
                    skillRangeD.transform.rotation = Quaternion.LookRotation(GetSkillRange(skillRanges[2]) - transform.position);
                }
                if (Input.GetKeyUp(KeyCode.D))
                {
                    skillRangeD.SetActive(false);
                    skillPosD = new Vector3(GetSkillRange(skillRanges[2]).x, 0.1f, GetSkillRange(skillRanges[2]).z);
                    transform.rotation = Quaternion.LookRotation(GetSkillRange(skillRanges[2]) - transform.position);
                    skillCooltimeD = skillMaxCooltimeD;
                    pv.RPC("PlayAnimation", RpcTarget.All, "StoneWall");
                }
            }
        }
    }

    public void OnUseSkillA()
    {
        if (pv.IsMine)
        {
            pv.RPC("UseStoneShoot", RpcTarget.All, null);
        }
    }
    public void OnUseSkillS()
    {
        if (pv.IsMine)
        {
            pv.RPC("UseEarthquake", RpcTarget.All, null);
        }
    }
    public void OnUseStoneWall()
    {
        if (pv.IsMine)
        {
            Quaternion fireRot = transform.rotation * Quaternion.Euler(new Vector3(0, 90, 0));
            pv.RPC("UseStoneWall", RpcTarget.All, skillPosD, fireRot);
        }
    }

    [PunRPC]
    void UseStoneShoot()
    {
        Quaternion fireRot = transform.rotation * Quaternion.Euler(new Vector3(90, 0, 0));
        GameObject fire = Instantiate(stonePrefab, transform.position + Vector3.up / 2, fireRot);
        fire.GetComponent<Stone>().targetPos = skillPosA;
    }

    [PunRPC]
    void UseEarthquake()
    {
        Quaternion fireRot = transform.rotation * Quaternion.Euler(new Vector3(0, 180, 0));
        GameObject fire = Instantiate(earthquakePrefab, skillPosS, fireRot);
    }
    [PunRPC]
    void UseStoneWall(Vector3 _position, Quaternion _rotation)
    {
        GameObject fire = Instantiate(stonewallPrefab, _position, _rotation);
        fire.GetComponent<StoneWall>().oriPos = _position;
        fire.GetComponent<StoneWall>().oriRot = _rotation;
    }
}
