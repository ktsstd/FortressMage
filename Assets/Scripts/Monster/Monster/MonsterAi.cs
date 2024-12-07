using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System.Security.Cryptography;

public class MonsterAI : MonoBehaviourPunCallbacks, IPunObservable
{
    public Transform player;
    public Transform skillTower;
    public Transform turret;
    public Transform castle;

    public float attackRange = 2.0f;
    public float attackCooldown = 2f;
    public float attackTimer = 0f;
    public float MaxHp = 20f;
    public float CurHp;
    public float defaultspped;
    public float monsterSlowCurTime;

    public int MonsterDmg = 10;

    public bool hasHealed = false; // 힐 상태 초기화
    public bool hasBuffed = false; 
    public bool NoTarget = false;
    public bool isSlow = false;
    public bool canMove = true;

    private List<(float slowtime, float slowmoveSpeed)> slowEffects = new List<(float, float)>();

    public NavMeshAgent agent;
    public Animator animator;
    protected PlayerController playerController;
    protected Skilltower skilltower;
    protected Turret turretS;
    protected Wall Castle;
    public AudioClip[] MonsterAudio;

    public LayerMask obstacleMask;

    public virtual void Start()
    {
        CurHp = MaxHp;

        skillTower = GameObject.FindWithTag("skilltower")?.transform;
        turret = GameObject.FindWithTag("turret")?.transform;
        castle = GameObject.FindWithTag("Castle")?.transform;
        player = GameObject.FindWithTag("Player")?.transform;

        NoTarget = false;
        hasBuffed = false;
        hasHealed = false;
        canMove = true;
        isSlow = false;

        // GameObject playerObject = GameObject.FindWithTag("Player");
        // if (playerObject != null)
        // {
        //     player = playerObject.transform;
        //     playerController = player.GetComponent<PlayerController>();
        // }        

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        obstacleMask = 1 << LayerMask.NameToLayer("Obstacle");
    }

    public virtual void Update()
    {
        Transform closestTarget = GetClosestTarget();

        if (closestTarget != null)
        {
            if (canMove)
            {
                float sqrDistanceToTarget = (closestTarget.position - transform.position).sqrMagnitude;

                if (sqrDistanceToTarget > attackRange * attackRange)
                {
                    agent.SetDestination(closestTarget.position);
                }
                else
                {
                    agent.ResetPath();

                    if (attackTimer <= 0f)
                    {
                        AttackTarget(closestTarget);
                        attackTimer = attackCooldown;
                    }
                }
            }

            else
            {
                agent.ResetPath();
            }
        }

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    private Coroutine BurnCoroutine;
    public void OnMonsterBurned(float _time, int burndmg)
    {
        if (BurnCoroutine != null)
            StopCoroutine(BurnCoroutine);

        BurnCoroutine = StartCoroutine(MonsterBurned(_time, burndmg));
    }

    IEnumerator MonsterBurned(float _time, int burndmg) // 스턴 상태 처리
    {
        while(_time <= 0)
        {
            _time -= 1f;
            MonsterDmged(burndmg);
            yield return new WaitForSeconds(1f);
        }
    }

    private Coroutine stunCoroutine;
    public virtual void OnMonsterStun(float _time)
    {
        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        stunCoroutine = StartCoroutine(MonsterStun(_time));
    }

    IEnumerator MonsterStun(float _time) // 스턴 상태 처리
    {
        canMove = false;
        animator.speed = 0f;
        yield return new WaitForSeconds(_time);
        canMove = true;
        animator.speed = 1f;
    }

    public virtual void OnMonsterSpeedDown(float _time, float _moveSpeed)
    {
        if (monsterSlowCurTime > 0)
        {
            slowEffects.Add((_time, _moveSpeed));
            slowEffects.Sort((a, b) => b.slowmoveSpeed.CompareTo(a.slowmoveSpeed));
        }
        else
        {
            OnMonsterSpeedDownStart(_time, _moveSpeed);
        }
    }

    private Coroutine speedCoroutine;
    public void OnMonsterSpeedDownStart(float _time, float _moveSpeed)
    {
        if (speedCoroutine != null)
            StopCoroutine(speedCoroutine);

        speedCoroutine = StartCoroutine(MonsterSpeedDowning(_time, _moveSpeed));
    }

    IEnumerator MonsterSpeedDowning(float _time, float _moveSpeed) // 이동 속도 감소 처리
    {
        isSlow = true;
        monsterSlowCurTime = _time;
        Speed = _moveSpeed;
        while (monsterSlowCurTime > 0)
        {
            yield return null;
            monsterSlowCurTime -= Time.deltaTime;

            for (int i = 0; i < slowEffects.Count; i++)
            {
                var remaineffect = slowEffects[i];
                slowEffects[i] = (remaineffect.slowtime - Time.deltaTime, remaineffect.slowmoveSpeed);
            }
        }
        // yield return new WaitForSeconds(_time);
        if (slowEffects.Count > 0)
        {
            var nextSlowEffect = slowEffects[0];
            slowEffects.RemoveAt(0);

            OnMonsterSpeedDownStart(nextSlowEffect.slowtime, nextSlowEffect.slowmoveSpeed);
        }
        else
        {
            if (!hasBuffed)
            {
                Speed = defaultspped;
            }
            else
            {
                Speed -= _moveSpeed;
            }
            speedCoroutine = null;
            isSlow = false;
        }
    }

    private Transform GetClosestTarget()
    {
        float closestSqrDistance = Mathf.Infinity;
        Transform closestTarget = null;

        string[] tags = { "skilltower", "turret", "Player" };

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
                    if (towerScript != null && towerScript.canAttack)
                    {
                        if (sqrDistanceToTarget < closestSqrDistance)
                        {
                            closestSqrDistance = sqrDistanceToTarget;
                            closestTarget = target;
                        }
                    }
                }
                else if (tag == "skilltower" || tag == "Player")
                {
                    if (sqrDistanceToTarget < closestSqrDistance)
                    {
                        closestSqrDistance = sqrDistanceToTarget;
                        closestTarget = target;
                    }
                }
            }
        }

        return closestTarget;
    }


    public virtual void AttackTarget(Transform target)
    {
        if (target == null) return;

        float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;
        if (sqrDistanceToTarget > attackRange * attackRange) return;

        if (target.CompareTag("Player"))
        {
            PlayerController playerScript = target.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                playerScript.OnHitPlayer(MonsterDmg);
            }
        }
        if (target.CompareTag("skilltower"))
        {
            Skilltower skillTowerScript = target.GetComponent<Skilltower>(); 
            if (skillTowerScript != null)
            {
                skillTowerScript.TakeDamage(MonsterDmg);
            }
        }
        if (target.CompareTag("Castle"))
        {
            Wall castleScript = target.GetComponent<Wall>();
            if (castleScript != null)
            {
                castleScript.TakeDamage(MonsterDmg);
            }
        }
        if (target.CompareTag("turret"))
        {
            Turret towerScript = target.GetComponent<Turret>();
            if (towerScript != null)
            {
                towerScript.TakeDamage(MonsterDmg);
            }
        }
    }


    public float AttackCooldown
    {
        get => attackCooldown;
        set => attackCooldown = Mathf.Max(0, value);
    }

    public float Speed
    {
        get => agent.speed;
        set
        {
            if (agent != null)
                agent.speed = value;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(CurHp);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            CurHp = (float)stream.ReceiveNext();
        }
    }

    public virtual void MonsterDmged(int playerDamage)
    {
        if (!photonView.IsMine) return;

        CurHp -= playerDamage;

        if (CurHp <= 0)
        {
            MonsterDied();
        }
    }

    [PunRPC]
    public void MonsterDied()
    {
        PhotonNetwork.Destroy(gameObject);
        GameManager.Instance.CheckMonsters();
    }
}
