using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricade : MonoBehaviour
{
    float damageDelay;

    void Start()
    {
        Invoke("SelfDestroy", 10f);
    }

    void Update()
    {
        if (damageDelay >= 0)
            damageDelay -= Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (damageDelay <= 0)
            {
                if (other.gameObject.TryGetComponent(out MonsterAI monster))
                {
                    monster.MonsterDmged(5);
                    monster.OnMonsterSpeedDown(3f, 2f);
                }

                damageDelay = 0.5f;
            }
        }
    }

    void ColliderOn()
    {
        GetComponent<SphereCollider>().enabled = true;
    }

    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
