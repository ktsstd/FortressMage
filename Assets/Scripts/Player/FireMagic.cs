using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMagic : PlayerController, ISkillAnimationEvent
{
    float[] skillRanges = { 10f, 10f, 10f };

    public GameObject skillRangeA;
    private Vector3 skillAPos;
    private float skillACooltime;
    public GameObject skillRangeS;
    
    public GameObject fireballPrefab;


    public override void Start()
    {
        base.Start();
    }
    public override void Update()
    {
        base.Update();
        if (pv.IsMine)
        {
            AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (aniInfo.IsName("A") || aniInfo.IsName("B") || aniInfo.IsName("C"))
                isCasting = true;
            else
                isCasting = false;

            if (skillACooltime >= 0) {skillACooltime -= Time.deltaTime;}
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
            stream.SendNext(skillAPos);
        }
        else
        {
            skillAPos = (Vector3)stream.ReceiveNext();
        }
    }

    void PlayerSkillA()
    {
        if (pv.IsMine)
        {
            if (skillACooltime <= 0)
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
                    skillAPos = new Vector3(GetSkillRange(skillRanges[0]).x, transform.position.y + 0.5f, GetSkillRange(skillRanges[0]).z);
                    skillACooltime = 3f;
                    pv.RPC("FireBallAni", RpcTarget.All, "FireBall");
                }
            }
        }
    }

    Vector3 GetSkillRange(float _range)
    {
        Vector3 direction = mousePosition - transform.position;
        float distance = direction.magnitude;

        if (distance > _range)
        {
            direction = direction.normalized;
            return transform.position + direction * _range;
        }
        else
            return mousePosition;
    }

    public void OnUseSkillA()
    {
        if (pv.IsMine)
        {
            pv.RPC("UseFireBall", RpcTarget.All, null);
        }
    }

    [PunRPC]
    void FireBallAni(string _ani)
    {
        animator.SetTrigger(_ani);
    }

    [PunRPC]
    void UseFireBall()
    {
        Quaternion fireRot = transform.rotation * Quaternion.Euler(new Vector3(0,180,0));
        GameObject fire = Instantiate(fireballPrefab, transform.position + Vector3.up / 2, fireRot);
        fire.GetComponent<FireBall>().targetPos = skillAPos;
    }
}
