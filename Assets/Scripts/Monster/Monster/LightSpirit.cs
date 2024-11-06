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

        // �������� �Ÿ� ��� �� ���ߴ� �Ÿ��� ��
        if (distanceToCastle > attackRange + stopDistance)
        {
            // �������� �̵� (��ֹ��� �ڵ����� ��ȸ)
            agent.speed = 10;
            agent.SetDestination(castleTransform.position); // ���� ��ġ�� �̵�
        }
        else if (distanceToCastle <= attackRange + stopDistance && distanceToCastle > attackRange)
        {
            agent.ResetPath(); // ��� �ʱ�ȭ�Ͽ� ����
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
        // ���� ������ �������ִ°�
        Destroy(this.gameObject);
        yield break;
    }

    public void MonsterDmged(int playerdamage)
    {
        if (CurHp <= 0) // ���� ü���� 0 ������ ��
        {
            CurHp -= playerdamage; // ü�� ����
        }
        else
        {
            Destroy(this.gameObject); // ���� ������Ʈ ����
        }
    }
}
