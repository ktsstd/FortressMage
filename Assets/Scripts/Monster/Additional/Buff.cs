using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    EliteMonsterLC eliteMonsterLC;
    MonsterAI monsterAI;

    public float speedBuff = 3f; // 버프 적용 시 속도 증가 비율
    public float attackCooldownReduction = 6f; // 쿨타임 감소량

    void Start()
    {
        eliteMonsterLC = gameObject.GetComponentInParent<EliteMonsterLC>();
        StartCoroutine(BuffRangeDisapear());
    }
    
    void OnTriggerEnter(Collider other)
    {
        MonsterAI monster = other.GetComponent<MonsterAI>();
        if (monster != null)
        {
            ApplyBuff(monster);
        }
    }

    void OnTriggerExit(Collider other)
    {
        MonsterAI monster = other.GetComponent<MonsterAI>();
        if (monster != null)
        {
            RemoveBuff(monster);
        }
    }

    private IEnumerator BuffRangeDisapear()
    {
        Debug.Log("60초 타이머시작");
        yield return new WaitForSeconds(60f);
        Debug.Log("끝");
        Destroy(this.gameObject);
    }

    private void ApplyBuff(MonsterAI monster)
    {
        monster.Speed *= speedBuff; // 몬스터 속도 증가
        monster.AttackCooldown -= attackCooldownReduction; // 공격 쿨타임 감소

        // 쿨타임이 0 미만이 되지 않도록 방지
        if (monster.AttackCooldown < 0)
        {
            monster.AttackCooldown = 0;
        }
    }

    private void RemoveBuff(MonsterAI monster)
    {
        monster.Speed /= speedBuff; // 원래 속도로 복구
        monster.AttackCooldown += attackCooldownReduction; // 원래 쿨타임으로 복구
    }
}
