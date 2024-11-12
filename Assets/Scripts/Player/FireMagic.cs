using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMagic : PlayerController, ISkillAnimationEvent
{
    Vector3 mousePosition;

    public GameObject skillRangeA;
    public GameObject skillRangeS;

    public GameObject fireballPrefab;

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
        }
        if (!isCasting)
        {
            PlayerSkillA();
            // PlayerSkillS();
            // PlayerSkillD();
        }
    }
    private Vector3 skillAPos;
    void PlayerSkillA()
    {
        float maxRange = 10f;


        if (Input.GetKey(KeyCode.A))
        {
            if (pv.IsMine)
            {
                GetMousePosition();

                Vector3 direction = mousePosition - transform.position;
                float distance = direction.magnitude;
                distance = Mathf.Min(distance, maxRange);
                skillRangeA.transform.position = transform.position + direction.normalized * (distance / 2);
                skillRangeA.transform.localScale = new Vector3(1f, 0.1f, distance) * 2;
                skillRangeA.transform.rotation = Quaternion.LookRotation(direction);
                skillAPos = transform.position + direction.normalized * distance;

                skillRangeA.SetActive(true);
            }
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            if (pv.IsMine)
            {
                skillRangeA.SetActive(false);
                Vector3 direction = mousePosition - transform.position;
                transform.rotation = Quaternion.LookRotation(direction);
                animator.SetTrigger("FireBall");
            }
        }
    }

    public void OnUseSkillA()
    {
        UseFireBall();

        pv.RPC("UseFireBall", RpcTarget.Others, null);
    }


    void GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));

        float distance;

        if (plane.Raycast(ray, out distance))
        {
            mousePosition = ray.GetPoint(distance);
        }
    }

    [PunRPC]
    void UseFireBall()
    {
        Quaternion fireRot = transform.rotation * Quaternion.Euler(new Vector3(0,180,0));
        GameObject fire = Instantiate(fireballPrefab, transform.position + Vector3.up / 2, fireRot);
        skillAPos = new Vector3(skillAPos.x, transform.position.y + 0.5f, skillAPos.z);
        fire.GetComponent<FireBall>().targetPos = skillAPos;
    }
}
