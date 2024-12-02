using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    private float speedBuff = 1.5f; // 버프 적용 시 속도 증가 비율
    private float attackCooldownReduction = 3f; // 쿨타임 감소량

    private void OnParticleCollision(GameObject other)
    {
        // 충돌한 GameObject가 MonsterAI를 가진 경우
        MonsterAI monster = other.GetComponent<MonsterAI>();
        if (monster != null)
        {
            Debug.Log("Particle collided with: " + other.name);  // 디버그 로그 출력
            ApplyBuff(monster);
        }
        else
        {
            Debug.Log("No MonsterAI found on: " + other.name);  // MonsterAI를 찾을 수 없는 경우
        }
        Debug.Log(other.name);  
    }

    private void ApplyBuff(MonsterAI monsterObj)
    {
        // 버프를 아직 받지 않은 몬스터만 처리
        if (!monsterObj.hasBuffed)
        {
            Debug.Log("Applying Buff to: " + monsterObj.gameObject.name);  // 버프가 적용될 때 로그 출력
            monsterObj.Speed *= speedBuff; // 몬스터 속도 증가
            monsterObj.AttackCooldown -= attackCooldownReduction; // 공격 쿨타임 감소
            monsterObj.hasBuffed = true; // 버프 적용 표시
        }
        else
        {
            Debug.Log("Already buffed: " + monsterObj.gameObject.name);  // 이미 버프가 적용된 경우
        }

        // 공격 쿨타임이 0 미만이 되지 않도록 방지
        if (monsterObj.AttackCooldown < 0)
        {
            monsterObj.AttackCooldown = 0;
        }
    }
}
