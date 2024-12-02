using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

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

    public int MonsterDmg = 10;

    public bool hasHealed = false; // 힐 상태 초기화

    public NavMeshAgent agent;
    protected PlayerController playerController;
    protected Skilltower skilltower;
    protected Turret turretS;
    protected Wall Castle;

    public LayerMask obstacleMask;

    public virtual void Start()
    {
        CurHp = MaxHp;

        skillTower = GameObject.FindWithTag("skilltower")?.transform;
        turret = GameObject.FindWithTag("turret")?.transform;
        castle = GameObject.FindWithTag("Castle")?.transform;
        player = GameObject.FindWithTag("Player")?.transform;

        // GameObject playerObject = GameObject.FindWithTag("Player");
        // if (playerObject != null)
        // {
        //     player = playerObject.transform;
        //     playerController = player.GetComponent<PlayerController>();
        // }        

        agent = GetComponent<NavMeshAgent>();
        obstacleMask = 1 << LayerMask.NameToLayer("Obstacle");
    }

    public virtual void Update()
    {
        Transform closestTarget = GetClosestTarget();

        if (closestTarget != null)
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

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    private Transform GetClosestTarget()
    {
        float closestSqrDistance = Mathf.Infinity;
        Transform closestTarget = null;

        Transform[] targets = { player, skillTower, turret, castle };

        foreach (Transform target in targets)
        {
            if (target == null) continue;

            // if (target.CompareTag("player"))
            // {
            //     PlayerController playerScript = target.GetComponent<PlayerController>();
            //     if (playerScript.isDie == false)
            //     {
            //         float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;
            //         if (sqrDistanceToTarget < closestSqrDistance)
            //         {
            //             closestSqrDistance = sqrDistanceToTarget;
            //             closestTarget = target;
            //         }
            //     }
            // }

            if (target.CompareTag("turret"))
            {
                Turret towerScript = target.GetComponent<Turret>();
                if (towerScript.canAttack == true)
                {
                    float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;
                    if (sqrDistanceToTarget < closestSqrDistance)
                    {
                        closestSqrDistance = sqrDistanceToTarget;
                        closestTarget = target;
                    }
                }
            }
            if (target.CompareTag("Castle"))
            {
                float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDistanceToTarget < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistanceToTarget;
                    closestTarget = target;
                }
            }

            if (target.CompareTag("skilltower"))
            {
                float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDistanceToTarget < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistanceToTarget;
                    closestTarget = target;
                }
            }

            if (target.CompareTag("player"))
            {
                float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDistanceToTarget < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistanceToTarget;
                    closestTarget = target;
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
