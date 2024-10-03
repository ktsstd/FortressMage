using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSpirit : MonoBehaviour
{
    public float speedBuff = 1.5f; // 버프 적용 시 속도 증가 비율
    public float attackCooldownReduction = 3f; // 쿨타임 감소량
    public float MaxHp = 30f;
    public float CurHp = 0f;

    private void Start()
    {
        CurHp = MaxHp;
    }

    private void OnTriggerEnter(Collider other)
    {
        MonsterAI monster = other.GetComponent<MonsterAI>();
        if (monster != null)
        {
            ApplyBuff(monster);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MonsterAI monster = other.GetComponent<MonsterAI>();
        if (monster != null)
        {
            RemoveBuff(monster);
        }
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

        Debug.Log($"{monster.name}이 버프 장판 안에 들어왔다");
    }

    private void RemoveBuff(MonsterAI monster)
    {
        monster.Speed /= speedBuff; // 원래 속도로 복구
        monster.AttackCooldown += attackCooldownReduction; // 원래 쿨타임으로 복구

        Debug.Log($"{monster.name}이 버프 장판에서 나갔다");
    }

    public void MonsterDmged(int playerdamage)
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
