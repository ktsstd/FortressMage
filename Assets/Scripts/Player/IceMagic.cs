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
        elementalCode = 2;
        playerAtk = 10;
        defaultAtk = 10;
        skillMaxCooltimeA = 4f;
        skillMaxCooltimeS = 6f;
        skillMaxCooltimeD = 20f;
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
                    skillCooltimeA = skillMaxCooltimeA;
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
                    skillCooltimeS = skillMaxCooltimeS;
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
                    skillCooltimeD = skillMaxCooltimeD;
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
            pv.RPC("UseFrozenAwl", RpcTarget.All, (int)playerAtk);
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
    void UseFrozenAwl(int _damage)
    {
        Quaternion fireRot = transform.rotation * Quaternion.Euler(new Vector3(0, 180, 0));
        GameObject ice = Instantiate(frozenawlPrefab, transform.position + Vector3.up / 2, fireRot);
        ice.GetComponent<FrozenAwl>().targetPos = skillPosA;
        ice.GetComponent<FrozenAwl>().damage = _damage;
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
            iceBullet.GetComponent<IceBullet>().damage = (int)(playerAtk / 2);

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

    public override void OffSkillRange()
    {
        skillRangeA.SetActive(false);
        skillRangeS.SetActive(false);
        skillRangeD.SetActive(false);
    }
}