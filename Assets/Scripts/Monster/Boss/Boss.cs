using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonsterAI
{
    private float[] BossMonsterSkillCooldowns = { 30f, 30f, 30f, 30f, 30f, 30f };
    public float[] BossMonsterSkillTimers = new float[6];  // 각 스킬의 남은 쿨타임을 관리하는 배열

    private float AllSkillCooldown = 5f;  // 전체 스킬 쿨타임
    public float AllSkillCooldownTimer;  // 전체 스킬 쿨타임 타이머

    private bool isBossPatern = false;  // 보스 패턴 활성화 여부

    public override void Start()
    {
        base.Start();  // 부모 클래스의 Start() 호출
        MaxHp = 200f;  // 체력 초기화
        MonsterDmg = 10;  // 몬스터 데미지 초기화
        CurHp = MaxHp;  // 체력 설정
        isBossPatern = false;
        GameObject playerObject = GameObject.FindWithTag("Player");  // "Player" 태그를 가진 오브젝트 찾기
        if (playerObject != null)
        {
            player = playerObject.transform;  // 플레이어의 위치 정보
        }
    }

    public override void Update()
    {
        base.Update();
        //float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        //if (distanceToPlayer > attackRange)
        //{
        //    if (!isBossPatern)
        //    {
        //        agent.SetDestination(player.position);
        //    }

        //    else
        //    {
        //        return;
        //    }
        //}
        //else
        //{
        //    StartCoroutine(BossPaternStart());
        //}
        StartCoroutine(BossPaternStart());

        // 각 스킬 타이머 갱신
        for (int i = 0; i < BossMonsterSkillTimers.Length; i++)
        {
            if (BossMonsterSkillTimers[i] > 0f)
            {
                BossMonsterSkillTimers[i] -= Time.deltaTime;
            }
        }

        if (AllSkillCooldownTimer > 0f)
        {
            AllSkillCooldownTimer -= Time.deltaTime;
        }
    }

    private int GetRandomSkill()
    {
        List<int> availableSkills = new List<int>();

        // 모든 스킬에 대해 쿨타임이 끝났으면 사용할 수 있는 스킬 목록에 추가
        for (int i = 0; i < BossMonsterSkillTimers.Length; i++)
        {
            if (BossMonsterSkillTimers[i] <= 0f)  // 쿨타임이 끝난 스킬만
            {
                availableSkills.Add(i);
            }
        }

        // 사용 가능한 스킬이 있다면 랜덤으로 선택
        if (availableSkills.Count > 0)
        {
            return availableSkills[Random.Range(0, availableSkills.Count)];
        }
        return -1;  // 사용할 수 있는 스킬이 없다면 -1 반환
    }

    private void UseSkill(int skillIndex)
    {
        switch (skillIndex)
        {
            case 0:
                StartCoroutine(BossSkill1());
                break;
            case 1:
                StartCoroutine(BossSkill2());
                break;
            case 2:
                StartCoroutine(BossSkill3());
                break;
            case 3:
                StartCoroutine(BossSkill4());
                break;
            case 4:
                StartCoroutine(BossSkill5());
                break;
            case 5:
                StartCoroutine(BossSkill6());
                break;
            default:
                Debug.LogWarning("Invalid skill index.");
                break;
        }
    }

    private IEnumerator BossSkill1()
    {
        Debug.Log("대충 손가락 튕기는 모션");
        // 검은다리 소환하는 코드 시작
        isBossPatern = false;
        yield break;
    }

    private IEnumerator BossSkill2()
    {
        Debug.Log("대충 해골투사체 발사하는 애니메이션");
        Debug.Log("대충 해골발사");
        Debug.Log("플레이어 기절 부르기");
        isBossPatern = false;
        yield break;
    }

    private IEnumerator BossSkill3()
    {
        Debug.Log("대충 눈에서 빛나는 애니메이션");
        Debug.Log("대충 어두운 연기 퍼져나가서 필드 덮는 이펙트");
        Debug.Log("대충 카메라 어두워짐"); // 바로 어두워짐 (카멘이아니라 녹턴같은느낌)
        isBossPatern = false;
        yield break;
    }

    private IEnumerator BossSkill4()
    {
        Debug.Log("대충 손가락으로 하늘 가르키는 애니메이션");
        Debug.Log("대충 어둠의창 떨어지는 이펙트");
        // 어둠의창 코드 시작
        isBossPatern = false;
        yield break;
    }

    private IEnumerator BossSkill5()
    {
        Debug.Log("대충 손바닥 부딫히는 애니메이션");
        // 플레이어 한곳으로 모으는 코드 시작
        Debug.Log("플레이어 속박 부르기");
        // 그 자리로 어둠구체 떨어트리는 코드 시작
        isBossPatern = false;
        yield break;
    }

    private IEnumerator BossSkill6()
    {
        Debug.Log("대충 일어나라 대사");
        // 죽었던 몬스터 되살리는 코드 << 현표한테 추가로 물어보기
        isBossPatern = false;
        yield break;
    }

    private IEnumerator BossPaternStart()
    {
        if (isBossPatern) yield break;
        isBossPatern = true;

        if (AllSkillCooldownTimer <= 0f)
        {
            int selectedSkill = GetRandomSkill();

            if (selectedSkill != -1)
            {
                BossMonsterSkillTimers[selectedSkill] = BossMonsterSkillCooldowns[selectedSkill];
                AllSkillCooldownTimer = AllSkillCooldown;

                UseSkill(selectedSkill);
            }
        }

        yield break;
    }
}
