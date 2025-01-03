using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class DarkSpirit : MonsterAI, IPunObservable
{
    private ParticleSystem particleSys;
    private Transform closestTarget;
    private Vector3 StartPosition;  // 몬스터의 초기 위치
    
    private GameObject TeleportEffect;

    private Vector3 SEffectpos;

    private float stopDistance = 0.8f;
    private bool Attacked = false;
    private bool isMoveStart = false;

    public override void Start()
    {
        base.Start();
        MaxHp = 40f;
        Speed = 10f;
        defaultspped = Speed;
        attackRange = 3f;
        CurHp = MaxHp;
        Attacked = false;
        isMoveStart = false;
        MonsterDmg = 15;

        particleSys = GetComponent<ParticleSystem>();
        StartPosition = transform.position;  // 몬스터의 초기 위치를 저장
    }

    public override void Update()
    {
        if (!Attacked && !NoTarget)
        {
            closestTarget = GetClosestTarget();
        }
        if (closestTarget == null)
        {
            NoTarget = true;
            GameObject castleObj = GameObject.FindWithTag("Castle");
            closestTarget = castleObj.transform;
        }

        float distanceTotarget = Vector3.Distance(transform.position, closestTarget.position);
        if (canMove && !Attacked)
        {
            if (distanceTotarget > attackRange + stopDistance)
            {
                animator.SetTrigger("StartMove");
                if (!isMoveStart)
                {
                    soundManager.PlayMonster(10, 1.0f, transform.position);
                    isMoveStart = true;
                }
                agent.SetDestination(closestTarget.position);
            }

            else
            {
                Attacked = true;
                agent.ResetPath();
                agent.velocity = Vector3.zero;
                StartCoroutine(DarkAttackStart());
            }
        }
        else if(!canMove)
        {
            agent.ResetPath();
            if (Attacked)
            {
                StopCoroutine(DarkAttackStart());
            }
        }
    }

    private Transform GetClosestTarget()
    {
        float closestSqrDistance = Mathf.Infinity;
        Transform closestTarget = null;

        string[] tags = { "skilltower", "turret" };

        foreach (string tag in tags)
        {
            GameObject[] targetsWithTag = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject targetObj in targetsWithTag)
            {
                Transform target = targetObj.transform;
                if (target == null) continue;

                float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;

                // 조건을 확인하여 가장 가까운 대상을 찾음
                if (tag == "turret")
                {
                    Turret towerScript = target.GetComponent<Turret>();
                    if (towerScript != null)
                    {
                        if (!towerScript.canAttack) continue;
                        else
                        {
                            if (sqrDistanceToTarget < closestSqrDistance)
                            {
                                closestSqrDistance = sqrDistanceToTarget;
                                closestTarget = target;
                            }
                        }
                    }
                }
                if (tag == "skilltower")
                {
                    Skilltower skilltowerScript = target.GetComponent<Skilltower>();
                    if (skilltowerScript != null)
                    {
                        if (!skilltowerScript.canAttack) continue;
                        else
                        {
                            if (sqrDistanceToTarget < closestSqrDistance)
                            {
                                closestSqrDistance = sqrDistanceToTarget;
                                closestTarget = target;
                            }
                        }
                    }
                }
            }
        }
        return closestTarget;
    }

    private IEnumerator DarkAttackStart()
    {
        Vector3 soundPosition = closestTarget.position;
        animator.SetTrigger("StartAttack");
        yield return new WaitForSeconds(0.5f);
        while(Attacked)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (stateInfo.IsName("Spirit of dark_attack"))
            {
                if (animTime >= 0.3)
                {
                    soundManager.PlayMonster(9, 1.0f, soundPosition);
                    yield return new WaitForSeconds(0.3f);
                    DarkDamageTarget(closestTarget);
                    Vector3 EffectPos = new Vector3(transform.position.x + 0.1f, transform.position.y - 2.5f, transform.position.z);
                    SEffectpos = EffectPos;
                    TeleportEffect = PhotonNetwork.Instantiate("Additional/Spirit of Dark_Teleport Effect", EffectPos, Quaternion.Euler(90, 0, 0));

                    TeleportEffect.transform.localPosition = EffectPos; 
                    TeleportEffect.transform.localRotation = Quaternion.identity;
                    
                    yield return new WaitForSeconds(2f);
                    StartCoroutine(DarkGoStart());
                    yield break;
                }

                else
                {
                    yield return null;
                }
            }
            else
            {
                yield return null;
            }
        }
    }
    void SoundStart()
    {
        soundManager.PlayMonster(11, 1.0f, SEffectpos);
    }

    private IEnumerator DarkGoStart()
    {
        float currentBaseOffset = agent.baseOffset;
        float targetBaseOffset = -0.3f;
        float GoStartTime = 0;
        float GoingTime = 1f;
        Invoke("SoundStart", 0.7f);
        while(GoStartTime < GoingTime)
        {
            agent.baseOffset = Mathf.Lerp(currentBaseOffset, targetBaseOffset, GoStartTime / GoingTime);
            GoStartTime += Time.deltaTime;
            yield return null;
        }
        transform.position = StartPosition;
        Vector3 EffectPos = new Vector3(transform.position.x + 0.1f, transform.position.y - 1.18f, transform.position.z);
        TeleportEffect = PhotonNetwork.Instantiate("Additional/Spirit of Dark_Teleport Effect", EffectPos, Quaternion.Euler(90, 0, 0));

        TeleportEffect.transform.localPosition = EffectPos; 
        TeleportEffect.transform.localRotation = Quaternion.identity;
        soundManager.PlayMonster(11, 1.0f, EffectPos);

        yield return new WaitForSeconds(1.9f);

        GoStartTime = 0;
        while(GoStartTime < GoingTime)
        {
            agent.baseOffset = Mathf.Lerp(targetBaseOffset, currentBaseOffset, GoStartTime / GoingTime);
            GoStartTime += Time.deltaTime;
            yield return null;
        }

        Attacked = false;
        isMoveStart = false;
    }

    private void DarkDamageTarget(Transform CurTarget)
    {
        if (CurTarget.CompareTag("skilltower"))
        {
            Skilltower skillTowerScript = CurTarget.GetComponent<Skilltower>();
            if (skillTowerScript != null)
            {
                skillTowerScript.TakeDamage(MonsterDmg);
            }
        }

        if (CurTarget.CompareTag("Castle"))
        {
            Wall castleScript = CurTarget.GetComponent<Wall>();
            if (castleScript != null)
            {
                castleScript.TakeDamage(MonsterDmg);
            }
        }

        if (CurTarget.CompareTag("turret"))
        {
            Turret towerScript = CurTarget.GetComponent<Turret>();
            if (towerScript != null)
            {
                towerScript.TakeDamage(MonsterDmg);
            }
        }
    }

    public override void MonsterDmged(int playerDamage)
    {
        if (!photonView.IsMine) return;

        CurHp -= playerDamage;
        Vector3 soundPosition = transform.position;
        soundManager.PlayMonster(20, 0.4f, soundPosition);
        StartCoroutine(MonsterFadeInOut());

        if (CurHp <= 0)
        {
            MonsterDied();
            Destroy(TeleportEffect);
        }
    }
}
