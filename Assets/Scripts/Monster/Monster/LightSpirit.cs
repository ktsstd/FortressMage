using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class LightSpirit : MonoBehaviourPun , IPunObservable
{
    private Animator animator;
    private Transform castleTransform;
    private NavMeshAgent agent;
    private LayerMask obstacleMask;

    private float attackRange = 1.0f;
    private float MaxHp = 30f;
    private float CurHp;
    private float stopDistance = 10.0f;
    // private float fadeDuration = 8.0f;

    private bool StartAttack = false;

    private void Start()
    {
        CurHp = MaxHp;
        StartAttack = false;
        animator = GetComponent<Animator>();
        GameObject castleObject = GameObject.FindWithTag("Castle");
        if (castleObject != null)
        {
            castleTransform = castleObject.transform;
        }

        else
        {
            return;
        }
        agent = GetComponent<NavMeshAgent>();
        obstacleMask = 1 << LayerMask.NameToLayer("Obstacle");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            MonsterDmged(30);
        }
        if (castleTransform == null) return;

        float distanceToCastle = Vector3.Distance(transform.position, castleTransform.position);

        if (distanceToCastle > attackRange + stopDistance)
        {
            agent.speed = 10;
            agent.SetDestination(castleTransform.position);
        }
        else if (distanceToCastle <= attackRange + stopDistance && distanceToCastle > attackRange)
        {
            agent.ResetPath();
            if (!StartAttack)
            {
                StartAttack = true;
                StartCoroutine(LightAttackStart());
            }
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

    private IEnumerator LightAttackStart()
    {
        yield return new WaitForSeconds(3f);
        animator.SetBool("StartAttack", true);
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
        yield break;
    }

    private IEnumerator StartDying()
    {
        // float elapsed = 0f;

        // // 점점 투명해지는 효과
        // while (elapsed < fadeDuration)
        // {
        //     elapsed += Time.deltaTime;
        //     float alpha = Mathf.Lerp(1f, -0.8f, elapsed / fadeDuration);

        //     foreach (MeshRenderer renderer in rendererObject)
        //     {
        //         renderer.material.SetFloat("_Tweak_transparency", alpha);
        //     }

        //     yield return null;
        // }

        Destroy(gameObject);
        yield break;
    }

    public void MonsterDmged(int playerdamage)
    {
        CurHp -= playerdamage; 
        if (agent != null && agent.enabled)
            {
                agent.isStopped = true;
            }
            animator.speed = 0f;
            StopCoroutine(LightAttackStart());
            StartCoroutine(StartDying());

        if (CurHp <= 0) // 현재 체력이 0 이하일 때
        {
            photonView.RPC("MonsterDied", RpcTarget.All);
        }
    }

    [PunRPC]
    public void LightSpiritDied()
    {

    }
}
