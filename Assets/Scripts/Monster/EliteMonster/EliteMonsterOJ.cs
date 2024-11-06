using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteMonsterOJ : MonsterAI
{
    private int MonsterShield = 60; // 몬스터의 쉴드 값
    private int CurShield = 0;

    // 스킬 쿨타임 설정
    private float skillCooldown = 5f; // 모든 스킬의 쿨타임
    private float detectionRange = 10f; // 벽 감지 범위
    private float skillTimer = 0f; // 스킬 타이머

    private float MaxHp40Per;

    private bool isShielded = false;
    private bool isShield = false;

    // Start() 함수: 몬스터가 시작될 때 호출, 초기화 진행
    public override void Start()
    {
        base.Start(); // 부모 클래스의 Start() 호출
        MaxHp = 200f;
        CurHp = MaxHp;
        MaxHp40Per = MaxHp * 0.4f;
        MonsterDmg = 10; // 몬스터 데미지 초기화
        GameObject playerObject = GameObject.FindWithTag("Player"); // "Player" 태그를 가진 오브젝트 찾기
        if (playerObject != null)
        {
            player = playerObject.transform; // 플레이어 변환 객체 가져오기
        }
    }

    // Update() 함수: 매 프레임마다 호출, 몬스터의 행동을 결정
    public override void Update()
    {
        if (player == null) return; // 플레이어가 없으면 동작 중지

        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 장애물이 감지되면
        if (IsObstacleBetweenPlayer())
        {
            // 스킬 쿨타임이 끝났을 경우 벽 깨기 스킬 사용
            if (skillTimer <= 0f)
            {
                StartCoroutine(EliteMonsterWallbreak());
                skillTimer = skillCooldown; // 스킬 쿨타임 설정
            }
            else
            {
                RaycastHit hit;
                Ray ray = new Ray(transform.position, player.position - transform.position);
                if (Physics.Raycast(ray, out hit, detectionRange, obstacleMask))
                {
                    if (hit.collider != null && hit.collider.CompareTag("Wall"))
                    {
                        StartCoroutine(LookAtObstacle(hit.normal, hit.point)); // 쿨타임 중에도 벽을 바라보기
                    }
                }
            }
        }
        else
        {
            // 플레이어가 공격 범위 밖에 있을 때
            if (distanceToPlayer > attackRange)
            {
                agent.SetDestination(player.position); // 플레이어를 향해 이동
            }
            else
            {
                agent.ResetPath(); // 플레이어가 공격 범위 안에 있으면 이동 멈춤

                // 공격 대기 시간이 0 이하일 때 플레이어 공격
                if (attackTimer <= 0f)
                {
                    EliteMonsterOJPattern(); // 몬스터의 스킬 사용
                    attackTimer = attackCooldown; // 공격 후 쿨타임 초기화
                }
            }
        }

        if (CurHp <= MaxHp40Per)
        {
            if (!isShielded)
            {
                Debug.Log("보호막 실행");
                StartCoroutine(EliteMonsterShield(MonsterShield));
                isShielded = true;
            }

            else
            {
                Debug.Log("보호막 이미 실행됨");
            }
        }

        if (CurShield > 0)
        {
            isShield = true;
            Debug.Log("쉴드생김");
        }

        else
        {
            isShield = false;
            // 대충 쉴드 깨지는 애니메이션 ( 넣는다면 )
        }

        // 공격 타이머와 스킬 쿨타임 감소
        attackTimer = Mathf.Max(0f, attackTimer - Time.deltaTime); // 쿨타임 감소
        skillTimer = Mathf.Max(0f, skillTimer - Time.deltaTime); // 쿨타임 감소
    }

    public override void MonsterDmged(int playerdamage)
    {
        if (isShield)
        {
            CurShield -= playerdamage;
        }

        else
        {
            CurHp -= playerdamage;
        }
    }

    // 벽을 바라보는 함수
    private IEnumerator LookAtObstacle(Vector3 hitNormal, Vector3 hitPoint)
    {
        agent.ResetPath(); // 이동 경로 리셋

        // 몬스터의 현재 위치에서 벽을 향하는 벡터 계산
        Vector3 directionToWall = (hitPoint - transform.position).normalized;
        directionToWall.y = 0; // y축 회전 방지 (바닥에 붙어서 회전)

        // 벽을 향하는 방향으로 회전하도록 수정
        Quaternion targetRotation = Quaternion.LookRotation(directionToWall);

        // 부드럽게 회전
        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
            yield return null; // 다음 프레임까지 대기
        }
    }

    // 플레이어와의 사이에 장애물이 있는지 확인
    private bool IsObstacleBetweenPlayer()
    {
        Ray ray = new Ray(transform.position, player.position - transform.position); // 플레이어 방향으로 레이 생성
        RaycastHit hit;

        // 레이캐스트를 통해 장애물 감지, 특정 레이어 마스크를 통해 벽만 감지
        if (Physics.Raycast(ray, out hit, detectionRange, obstacleMask))
        {
            // 감지된 객체가 벽인지 확인
            if (hit.collider.CompareTag("Wall"))
            {
                return true; // 벽이 있는 경우 true 반환
            }
        }
        return false; // 벽이 없는 경우 false 반환
    }

    // 몬스터의 스킬 선택 로직
    private void EliteMonsterOJPattern()
    {
        if (skillTimer <= 0f) // 스킬 쿨타임이 끝났을 경우
        {
            if (IsObstacleBetweenPlayer())
            {
                StartCoroutine(EliteMonsterWallbreak());
            }
            else
            {
                StartCoroutine(EliteMonsterStamp(MonsterDmg));
            }
            skillTimer = skillCooldown; // 쿨타임 설정
        }
        else
        {
            Invoke("EliteMonsterOJPattern", 2f);
        }
    }

    // 첫 번째 스킬: 플레이어에게 데미지를 입히는 코루틴
    private IEnumerator EliteMonsterStamp(int MonsterDmg)
    {
        yield return new WaitForSeconds(2f); // 2초 대기
        // playercontroller.PlayerCurHealth(MonsterDmg); // 플레이어의 체력을 감소시킴
    }

    // 두 번째 스킬: 몬스터에게 쉴드를 주는 코루틴
    private IEnumerator EliteMonsterShield(int MonsterShield)
    {
        agent.ResetPath();
        yield return new WaitForSeconds(2f); // 2초 대기
        Debug.Log("쉴드");
        CurShield = MonsterShield;
    }

    // 세 번째 스킬: 벽을 깨는 코루틴
    private IEnumerator EliteMonsterWallbreak()
    {
        yield return new WaitForSeconds(2f); // 2초 대기

        // 플레이어 방향으로 레이 생성 (시작 지점을 몬스터 위치에서 더 멀리 설정)
        Vector3 directionToPlayer = (player.position - transform.position).normalized; 
        Vector3 rayStartPosition = transform.position + directionToPlayer * 1f; 
        Ray ray = new Ray(rayStartPosition, directionToPlayer); 
        RaycastHit hit; 

        // 벽을 부수는 레이캐스트
        if (Physics.Raycast(ray, out hit, detectionRange + 3f, obstacleMask))
        {
            // 벽이 발견된 경우
            if (hit.collider.CompareTag("Wall"))
            {
                Debug.Log("벽 발견: " + hit.collider.name); 

                // 벽의 위치와 법선으로 돌진 목표를 설정
                Vector3 wallPosition = hit.point; 
                Vector3 dashTarget = wallPosition - (hit.normal * 0.5f); 

                Debug.Log("돌진 목표 위치: " + dashTarget); 

                // 벽을 바라보도록 하는 호출 추가
                yield return StartCoroutine(LookAtObstacle(hit.normal, hit.point)); 

                float dashSpeed = 40f; // 돌진 속도

                // 몬스터가 벽을 향해 돌진하는 로직
                while (Vector3.Distance(transform.position, dashTarget) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, dashTarget, dashSpeed * Time.deltaTime);

                    // 벽에 닿았는지 체크
                    if (Vector3.Distance(transform.position, wallPosition) < 2.5f)
                    {
                        Destroy(hit.collider.gameObject); // 벽을 즉시 파괴
                        Debug.Log("벽이 즉시 파괴되었습니다."); // 벽 파괴 로그
                        yield break; // 코루틴 종료
                    }

                    yield return null; // 다음 프레임까지 대기
                }

                Debug.Log("돌진 완료");
                EliteMonsterOJPattern();
            }
        }
        else
        {
            Debug.Log("벽이 발견되지 않았습니다."); 
        }
    }
}
