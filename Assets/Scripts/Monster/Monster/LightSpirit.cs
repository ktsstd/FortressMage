using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class LightSpirit : MonsterAI, IPunObservable
{
    private Animator animator;
    private Transform castleTransform;
    private ParticleSystem particleSys;

    private float stopDistance = 10.0f;
    private float LightMonsterSpeed = 5.0f;
    // private float fadeDuration = 8.0f;

    private bool StartAttack = false;

    public override void Start()
    {
        base.Start();
        MaxHp = 20f;
        agent.speed = LightMonsterSpeed;
        CurHp = MaxHp;
        StartAttack = false;
        animator = GetComponent<Animator>();
        particleSys = GetComponentInChildren<ParticleSystem>();
        GameObject castleObject = GameObject.FindWithTag("Castle");
        if (castleObject != null)
        {
            castleTransform = castleObject.transform;
        }

        else
        {
            return;
        }
    }

    public override void Update()
    {
        if (castleTransform == null) return;

        float distanceToCastle = Vector3.Distance(transform.position, castleTransform.position);

        if (distanceToCastle > attackRange + stopDistance)
        {
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

    private IEnumerator LightAttackStart()
    {
        yield return new WaitForSeconds(3f);
        animator.SetBool("StartAttack", true);
        particleSys.Play();
        yield return new WaitForSeconds(3f);
        // photonView.RPC("성벽 데미지 주는거", RpcTarget.All);
        yield return new WaitForSeconds(2f);
        photonView.RPC("MonsterDied", RpcTarget.All);
        yield break;
    }
}
