using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EliteMonsterLC : MonoBehaviour
{
    public float speedBuff = 3f; // 버프 적용 시 속도 증가 비율
    public float attackCooldownReduction = 6f; // 쿨타임 감소량
    public float moveSpeed = 10f;
    public float AllSkillCooldown = 10f;
    public float MaxHp = 100f;
    public float CurHp = 0f;
    
    public float skill1Cooldown = 10f;
    public float skill2Cooldown = 60f;
    public float skill3Cooldown = 90f;

    public float skill1CooldownTimer;
    public float skill2CooldownTimer;
    public float skill3CooldownTimer;
    public float AllSkillCooldownTimer;

    public GameObject BuffRange;

    public Transform target;
    public NavMeshAgent agent;



    // Start is called before the first frame update
    void Start()
    {
        // GameObject playerObject = GameObject.FindWithTag("Player");
        // player = playerObject.transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        CurHp = MaxHp;
    }

    // Update is called once per frame
    void Update()
    {
        skill1CooldownTimer = Mathf.Max(0f, skill1CooldownTimer - Time.deltaTime);
        skill2CooldownTimer = Mathf.Max(0f, skill2CooldownTimer - Time.deltaTime);
        skill3CooldownTimer = Mathf.Max(0f, skill3CooldownTimer - Time.deltaTime);

        AllSkillCooldownTimer = Mathf.Max(0f, AllSkillCooldownTimer - Time.deltaTime);
        
        if (target != null)
        {
            agent.SetDestination(target.position);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    EliteMonsterLCPattern();
                }
            }
        }

        else
        {
            EliteMonsterLCPattern();
        }
    }

    private void EliteMonsterLCPattern()
    {
        if (AllSkillCooldownTimer <= 0f)
        {
            int RandomSkill = Random.Range(0, 3);
            switch(RandomSkill)
            {
                case 0:
                    if (skill1CooldownTimer <= 0f)
                    {
                        StartCoroutine(EliteMonsterHealDelay());
                        Debug.Log("1번스킬 힐");
                        skill1CooldownTimer = skill1Cooldown; // 스킬 1 쿨타임 시작
                        AllSkillCooldownTimer = AllSkillCooldown; // 전체 스킬 쿨타임 시작
                    }
                    break;

                case 1:
                    if (skill2CooldownTimer <= 0f)
                    {
                        StartCoroutine(EliteMonsterStun());
                        skill2CooldownTimer = skill2Cooldown; // 스킬 2 쿨타임 시작
                        AllSkillCooldownTimer = AllSkillCooldown; // 전체 스킬 쿨타임 시작
                    }
                    break;

                case 2:
                    if (skill3CooldownTimer <= 0f)
                    {
                        StartCoroutine(EliteMonsterBuff());
                        Debug.Log("3번스킬 버프장판");
                        skill3CooldownTimer = skill3Cooldown; // 스킬 3 쿨타임 시작
                        AllSkillCooldownTimer = AllSkillCooldown; // 전체 스킬 쿨타임 시작
                    }
                    break;
            }
        }
    }

    private IEnumerator EliteMonsterHealDelay()
    {
        yield return new WaitForSeconds(2f);
        EliteMonsterHeal();
    }

    private void EliteMonsterHeal()
    {   
        MonsterAI[] monsters = FindObjectsOfType<MonsterAI>(); // 모든 MonsterAI 오브젝트 찾기
        bool anyHealed = false; // 힐이 발생했는지 확인하는 플래그

        foreach (MonsterAI heal in monsters)
        {
            if (!heal.hasHealed) // 힐을 아직 받지 않은 경우
            {
                float MonsterMaxHp = heal.MaxHp; // 최대 체력
                float MonsterCurHp = heal.CurHp; // 현재 체력
                float lostHp = MonsterMaxHp - MonsterCurHp; // 잃은 체력

                if (lostHp > 0) // 잃은 체력이 있을 때만 회복
                {
                    float healHp = lostHp * 0.05f; // 잃은 체력의 5% 계산
                    float newCurHp = Mathf.Min(MonsterCurHp + healHp, MonsterMaxHp); // 최대 체력 초과 방지

                    // 로그 출력
                    Debug.Log("몬스터: " + heal.gameObject.name);
                    Debug.Log("현재 체력: " + MonsterCurHp);
                    Debug.Log("최대 체력: " + MonsterMaxHp);
                    Debug.Log("회복해야 할 값: " + healHp);
                    Debug.Log("회복 후 남은 체력: " + newCurHp);

                    // 현재 체력을 회복한 값으로 업데이트
                    heal.CurHp = newCurHp;

                    // 힐을 받았으므로 hasHealed를 true로 설정
                    heal.hasHealed = true;
                    anyHealed = true; // 힐이 발생했음을 기록
                }
                else
                {
                    Debug.Log("몬스터: " + heal.gameObject.name + "는 체력이 가득 찼습니다.");
                }
            }
            else
            {
                Debug.Log("몬스터: " + heal.gameObject.name + "는 이미 힐을 받았습니다.");
            }
        }
        
        // 힐이 발생한 경우에만 리셋
        if (anyHealed)
        {
            StartCoroutine(ResetHealAfterDelay(9f));
        }
    }

    private IEnumerator ResetHealAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기

        MonsterAI[] monsters = FindObjectsOfType<MonsterAI>(); // 모든 MonsterAI 오브젝트 찾기

        foreach (MonsterAI heal in monsters)
        {
            heal.hasHealed = false; // 힐 상태를 리셋
        }
    }

    private IEnumerator EliteMonsterStun()
    {
        Debug.Log("기절~");
        // 대충 기절 태성화이팅
        yield break;
    }

    private IEnumerator EliteMonsterBuff()
    {
        Debug.Log("보스에서 빛이 난다");
        yield return new WaitForSeconds(2f);
        Debug.Log("보스가 주문을 외친다");
        yield return new WaitForSeconds(6f);
        Debug.Log("버프장판 생성");
        if (target != null)
        {
            Vector3 buffspawnposition = new Vector3(target.position.x, target.position.y - 0.56f, target.position.z);
            Instantiate(BuffRange, buffspawnposition, Quaternion.identity);
        }
        else
        {
            Vector3 buffspawnpositionNoTarget = new Vector3(transform.position.x, transform.position.y - 0.56f, transform.position.z);
            Instantiate(BuffRange, buffspawnpositionNoTarget, transform.rotation);
        }
        yield break;
    }

    public virtual void MonsterDmged(int playerdamage)
    {
        if (CurHp <= 0)
        {
            CurHp -= playerdamage;
        }

        else
        {
            Destroy(this.gameObject);
        }
    }
}

