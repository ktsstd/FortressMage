using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceRoad : MonoBehaviour
{
    float damageDelay;
    float summonTime;
    private void Start()
    {
        if (gameObject.name == "IceRoad")
            summonTime = 3.1f;
        else
            summonTime = 5.1f;

        Invoke("SelfDestroy", summonTime);
    }
    private void OnTriggerStay(Collider other)
    {
        if (damageDelay >= 0)
            damageDelay -= Time.deltaTime;

        if (other.tag == "Enemy")
        {
            if (damageDelay <= 0)
            {
                if (other.gameObject.TryGetComponent(out MonsterAI monster))
                    monster.OnMonsterSpeedDown(1f, 2.5f);

                damageDelay = 1f;
            }
        }
    }
    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
