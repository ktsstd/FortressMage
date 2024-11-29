using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class DarkSpirit : MonsterAI, IPunObservable
{
    // private Animator animator;
    // // private Transform castleTransform;
    // private ParticleSystem particleSys;
    
    // private Transform closestTarget;

    // private float stopDistance = 2.0f;
    // private float LightMonsterSpeed = 5.0f;
    // // private float fadeDuration = 8.0f;

    // private bool StartAttack = false;

    // public override void Start()
    // {
    //     base.Start();
    //     MaxHp = 400f;
    //     Speed = 5f;
    //     CurHp = MaxHp;
    //     animator = GetComponent<Animator>();
    //     particleSys = GetComponentInChildren<ParticleSystem>();
    //     MonsterDmg = 50;
    //     // GameObject castleObject = GameObject.FindWithTag("Castle");
    //     // if (castleObject != null)
    //     // {
    //     //     castleTransform = castleObject.transform;
    //     // }

    //     // else
    //     // {
    //     //     return;
    //     // }
    // }

    // public override void Update()
    // {
    //     // if (castleTransform == null) return;
    //     if (StartAttack) return;
    //     closestTarget = GetClosestTarget();
        
    //     float distanceTotarget = Vector3.Distance(transform.position, closestTarget.position);

    //     if (distanceTotarget > attackRange + stopDistance)
    //     {
    //         agent.SetDestination(closestTarget.position);
    //     }
    //     else if (distanceTotarget <= attackRange + stopDistance && distanceTotarget > attackRange)
    //     {
    //         agent.ResetPath();
    //         if (!StartAttack)
    //         {
    //             StartAttack = true;
    //             StartCoroutine(LightAttackStart());
    //         }
    //     }
    // }

    // private Transform GetClosestTarget()
    // {
    //     float closestSqrDistance = Mathf.Infinity;
    //     Transform closestTarget = null;

    //     // "Turret", "Castle", "SkillTower"와 같은 태그를 사용해 각 대상을 찾음
    //     string[] tags = { "skilltower", "turret", "Castle" };

    //     foreach (string tag in tags)
    //     {
    //         // 해당 태그를 가진 모든 객체를 찾음
    //         GameObject[] targetsWithTag = GameObject.FindGameObjectsWithTag(tag);

    //         foreach (GameObject targetObj in targetsWithTag)
    //         {
    //             // 각 타겟의 Transform을 가져옴
    //             Transform target = targetObj.transform;

    //             // 타겟이 null이 아닌지 확인
    //             if (target == null) continue;

    //             // 현재 객체와 타겟의 거리 계산 (제곱 거리 사용)
    //             float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;

    //             // 가장 가까운 타겟을 선택
    //             if (sqrDistanceToTarget < closestSqrDistance)
    //             {
    //                 closestSqrDistance = sqrDistanceToTarget;
    //                 closestTarget = target;
    //             }
    //         }
    //     }

    //     return closestTarget;
    // }


    // private IEnumerator LightAttackStart()
    // {
    //     yield return new WaitForSeconds(3f);
    //     animator.SetBool("StartAttack", true);
    //     particleSys.Play();
    //     yield return new WaitForSeconds(2f);
    //     LightDamageTarget(closestTarget);
    //     yield return new WaitForSeconds(2f);
    //     photonView.RPC("MonsterDied", RpcTarget.All);
    //     yield break;
    // }

    // private void LightDamageTarget(Transform CurTarget)
    // {
    //     if (CurTarget.CompareTag("skilltower"))
    //     {
    //         Skilltower skillTowerScript = CurTarget.GetComponent<Skilltower>(); 
    //         if (skillTowerScript != null)
    //         {
    //             skillTowerScript.TakeDamage(MonsterDmg);
    //         }
    //     }

    //     if (CurTarget.CompareTag("Castle"))
    //     {
    //         Wall castleScript = CurTarget.GetComponent<Wall>();
    //         if (castleScript != null)
    //         {
    //             castleScript.TakeDamage(MonsterDmg);
    //         }
    //     }
        
    //     if (CurTarget.CompareTag("turret"))
    //     {
    //         Turret towerScript = CurTarget.GetComponent<Turret>();
    //         if (towerScript != null)
    //         {
    //             towerScript.TakeDamage(MonsterDmg);
    //         }
    //     }
    // }
}
