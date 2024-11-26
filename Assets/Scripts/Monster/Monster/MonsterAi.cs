using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Unity.VisualScripting;

public class MonsterAI : MonoBehaviourPunCallbacks, IPunObservable
{
    // 플레이어의 위치를 참조하는 변수
    public Transform player;
    public float attackRange = 2.0f;  // 몬스터의 공격 범위
    public float attackCooldown = 2f; // 공격 후 쿨타임
    public float attackTimer = 0f;    // 공격 대기 시간 타이머
    public float MaxHp = 30f;         // 몬스터의 최대 체력
    public float CurHp;               // 몬스터의 현재 체력
    public bool hasHealed;            // 힐 여부 체크

    public int MonsterDmg = 10;        // 몬스터의 공격력

    // NavMeshAgent 컴포넌트 (네비메시를 통한 이동)
    public NavMeshAgent agent;
    protected PlayerController playercontroller; // 플레이어 컨트롤러 참조

    // 장애물 감지용 레이어 마스크
    public LayerMask obstacleMask;

    // Start() 함수: 몬스터가 시작될 때 호출, 초기화 진행
    public virtual void Start()
    {
        CurHp = MaxHp; // 현재 체력을 최대 체력으로 초기화
        hasHealed = false; // 힐 상태 초기화
        // "Player" 태그를 가진 게임 오브젝트를 찾아 플레이어 참조
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform; // 플레이어의 위치 설정
            // 플레이어의 체력을 컨트롤하기 위한 컴포넌트 참조
            playercontroller = player.GetComponent<PlayerController>();
        }
        // NavMeshAgent 컴포넌트를 가져와 몬스터가 경로를 계산해 이동할 수 있도록 설정
        agent = GetComponent<NavMeshAgent>();
        obstacleMask = 1 << LayerMask.NameToLayer("Obstacle"); // 장애물 레이어 마스크 설정
    }

    public virtual void Update()
    {
        if (player == null) return; // 플레이어가 없으면 동작 중지

        // 모든 플레이어를 찾아서 가장 가까운 플레이어를 선택
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = Mathf.Infinity;
        Transform closestPlayer = null;

        foreach (GameObject p in players)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, p.transform.position);
            if (distanceToPlayer < closestDistance)
            {
                closestDistance = distanceToPlayer;
                closestPlayer = p.transform;
            }
        }

        // 가장 가까운 플레이어를 목표로 설정
        if (closestPlayer != null)
        {
            player = closestPlayer;
        }

        // 플레이어와의 거리 계산
        float distanceToTarget = Vector3.Distance(transform.position, player.position);

        // 플레이어가 공격 범위 밖에 있을 때
        if (distanceToTarget > attackRange)
        {
            // 플레이어를 향해 이동 (장애물은 자동으로 우회)
            agent.SetDestination(player.position);
        }
        else
        {
            // 플레이어가 공격 범위 안에 있으면 이동 멈춤
            agent.ResetPath();

            // 공격 대기 시간이 0 이하일 때 플레이어 공격
            if (attackTimer <= 0f)
            {
                AttackPlayer(MonsterDmg); // 플레이어에게 데미지 입힘
                attackTimer = attackCooldown; // 공격 후 쿨타임 초기화
            }
        }

        // 공격 타이머를 줄임 (쿨타임이 끝날 때까지 기다림)
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime; // 쿨타임 감소
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

    // AttackCooldown 프로퍼티: 공격 쿨타임을 관리
    public float AttackCooldown
    {
        get { return attackCooldown; }
        set
        {
            // 쿨타임이 음수일 경우 예외 처리
            if (value < 0)
            {
                return;
            }
            attackCooldown = value; // 쿨타임 설정
        }
    }

    // Speed 프로퍼티: NavMeshAgent의 이동 속도를 설정
    public float Speed
    {
        get { return agent.speed; }
        set
        {
            if (agent != null)
            {
                agent.speed = value; // agent가 null이 아닐 때만 속도 설정
            }
        }
    }

    // 플레이어와의 사이에 장애물이 있는지 확인
    private bool IsObstacleBetweenPlayer()
    {
        Ray ray = new Ray(transform.position, player.position - transform.position); // 플레이어 방향으로 레이 생성
        RaycastHit hit;

        // 레이캐스트를 통해 장애물 감지
        if (Physics.Raycast(ray, out hit, attackRange, obstacleMask))
        {
            return hit.collider != null; // 장애물이 있는 경우 true 반환
        }

        return false; // 장애물이 없는 경우 false 반환
    }

    // 플레이어를 공격하는 함수
    public virtual void AttackPlayer(int damage)
    {
        // playercontroller.PlayerCurHealth(damage); // 플레이어의 체력을 감소시킴
    }

    // 몬스터가 피해를 입었을 때 호출되는 함수
    public virtual void MonsterDmged(int playerdamage)
    {
        if (!photonView.IsMine) return;
        
        CurHp -= playerdamage; // 체력 감소

        if (CurHp <= 0) // 현재 체력이 0 이하일 때
        {
            photonView.RPC("MonsterDied", RpcTarget.All);
        }
    }
    
    [PunRPC]
    public void MonsterDied()
    {
        PhotonNetwork.Destroy(this.gameObject);
        GameManager.Instance.CheckMonsters();
    }
}
