using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LightSpirit : MonoBehaviour
{
    Animator animator;

    private Transform castleTransform;

    private NavMeshAgent agent;

    private LayerMask obstacleMask;

    private float attackRange = 1.0f;
    private float MaxHp = 30f;
    private float CurHp;
    private float stopDistance = 15.0f;

    private bool StartAttack = false;

    private void Start()
    {
        CurHp = MaxHp;
        StartAttack = false;
        animator = GetComponent<Animator>();
        //StartCoroutine(LightAttackStart());
        GameObject castleObject = GameObject.FindWithTag("Castle");
        if (castleObject != null)
        {
            castleTransform = castleObject.transform;
            //agent.SetDestination(castleTransform.position);
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

        // 성벽과의 거리 계산 후 멈추는 거리와 비교
        if (distanceToCastle > attackRange + stopDistance)
        {
            // 성벽으로 이동 (장애물은 자동으로 우회)
            agent.speed = 10;
            agent.SetDestination(castleTransform.position); // 성벽 위치로 이동
        }
        else if (distanceToCastle <= attackRange + stopDistance && distanceToCastle > attackRange)
        {
            agent.ResetPath(); // 경로 초기화하여 멈춤
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
        animator.SetTrigger("StartAttack");
        yield return new WaitForSeconds(5f);
        // 대충 성벽에 데미지주는거
        Destroy(this.gameObject);
        yield break;
    }

    public void MonsterDmged(int playerdamage)
    {
        if (CurHp <= 0) // 현재 체력이 0 이하일 때
        {
            CurHp -= playerdamage; // 체력 감소
        }
        else
        {
            Destroy(this.gameObject); // 몬스터 오브젝트 삭제
        }
    }
}
