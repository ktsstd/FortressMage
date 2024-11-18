using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class DarkSpirit : MonoBehaviour
{
    private Animator animator;
    private Transform skilltowerTransform;
    private NavMeshAgent agent;
    private float stopDistance = 9f;
    private float attackRange = 1.0f;
    private float CurHp;
    private float MaxHp = 100f;

     void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        CurHp = MaxHp;
        GameObject skilltowerObject = GameObject.FindWithTag("skilltower");
        if (skilltowerObject != null)
        {
            skilltowerTransform = skilltowerObject.transform;
        }
    }

    void Update()
    {
        if (skilltowerTransform == null) return;

        float distanceToCastle = Vector3.Distance(transform.position, skilltowerTransform.position);

        if (distanceToCastle > attackRange + stopDistance)
        {
            agent.speed = 10;
            animator.SetTrigger("StartAttack");
            agent.SetDestination(skilltowerTransform.position);
        }
        else if (distanceToCastle <= attackRange + stopDistance && distanceToCastle > attackRange)
        {
            agent.ResetPath();
            StartCoroutine(DarkSpiritAttackStart());
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 이 클라이언트에서 몬스터 위치와 회전을 전송
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(CurHp);
        }
        else
        {
            // 다른 클라이언트에서 몬스터 위치와 회전을 수신
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            CurHp = (float)stream.ReceiveNext();
        }
    }

    private IEnumerator DarkSpiritAttackStart()
    {
        yield break;
    }
}
