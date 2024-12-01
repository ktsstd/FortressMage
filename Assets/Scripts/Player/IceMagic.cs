using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceMagic : PlayerController
{
    float[] skillRanges = { 10f, 10f, 12f };

    public GameObject skillRangeA;
    private Vector3 skillPosA;
    private float skillCooltimeA;

    public GameObject skillRangeS;
    private Vector3 skillPosS;
    private float skillCooltimeS;

    public GameObject skillRangeD;
    private Vector3 skillTargetPosD;
    private Vector3 skillSummonPosD;
    private float skillCooltimeD;

    public GameObject blizzardPrefab;
    public GameObject frozenawlPrefab;
    public GameObject frostShacklesPrefab;
    public GameObject[] iceBulletPrefab;

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
            stream.SendNext(skillTargetPosD);
            stream.SendNext(skillSummonPosD);
        }
        else
        {
            skillPosA = (Vector3)stream.ReceiveNext();
            skillPosS = (Vector3)stream.ReceiveNext();
            skillTargetPosD = (Vector3)stream.ReceiveNext();
            skillSummonPosD = (Vector3)stream.ReceiveNext();
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
                    transform.rotation = Quaternion.LookRotation(GetSkillRange(skillRanges[1]) - transform.position);
                    skillPosS = new Vector3(GetSkillRange(skillRanges[1]).x, 0.1f, GetSkillRange(skillRanges[1]).z);
                    skillCooltimeS = 5f;
                    pv.RPC("PlayAnimation", RpcTarget.All, "FrostShackles");
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
                    skillCooltimeD = 10f;
                    skillTargetPosD = new Vector3(GetSkillRange(skillRanges[2]).x, 0.1f, GetSkillRange(skillRanges[2]).z);
                    transform.rotation = Quaternion.LookRotation(GetSkillRange(skillRanges[2]) - transform.position);
                    pv.RPC("PlayAnimation", RpcTarget.All, "Blizzard");
                    pv.RPC("UseBlizzard", RpcTarget.All, null);
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

    public void OnUseSkillS()
    {
        if (pv.IsMine)
        {
            pv.RPC("UseFrostShackles", RpcTarget.All, null);
        }
    }

    [PunRPC]
    void UseFrozenAwl()
    {
        Quaternion fireRot = transform.rotation * Quaternion.Euler(new Vector3(0, 180, 0));
        GameObject ice = Instantiate(frozenawlPrefab, transform.position + Vector3.up / 2, fireRot);
        ice.GetComponent<FrozenAwl>().targetPos = skillPosA;
    }

    [PunRPC]
    void UseFrostShackles()
    {
        GameObject frostShackles = Instantiate(frostShacklesPrefab, skillPosS, transform.rotation);
    }

    [PunRPC]
    void UseBlizzard()
    {
        skillSummonPosD = transform.position + Vector3.up * 8;
        skillTargetPosD = GetSkillRange(skillRanges[2]);
        Instantiate(blizzardPrefab, skillTargetPosD, transform.rotation * Quaternion.Euler(new Vector3(0, 180, 0)));

        StartCoroutine("Blizzard");
    }

    IEnumerator Blizzard()
    {
        int numberOfObjects = 10;
        for (int i = 0; i < numberOfObjects; i++)
        {

            float angle = i * Mathf.PI * 2 / numberOfObjects;

            Vector3 spawnPosition = new Vector3(Mathf.Cos(angle) * 1f, 0, Mathf.Sin(angle) * 1f) + skillSummonPosD;

            GameObject iceBullet = Instantiate(iceBulletPrefab[Random.Range(0, 6)], spawnPosition, transform.rotation * Quaternion.Euler(new Vector3(0, 180, 0)));
            iceBullet.GetComponent<IceBullet>().targetPos = GetAttackPoint(skillTargetPosD, i);

            yield return new WaitForSeconds(0.3f);
        }
    }

    Vector3 GetAttackPoint(Vector3 _center, int _i)
    {
        float radiusOffset = 1.3f * _i;

        if (_i >= 5)
            radiusOffset = 1f * (5 - (_i - 5));

        float angleOffset = 70 * _i * (_i % 2 == 0 ? 1 : -1);

        float angle = angleOffset * Mathf.Deg2Rad;
        float radius = radiusOffset;

        Vector3 spawnPosition = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius) + _center;

        return spawnPosition;
    }
}