using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRoad : MonoBehaviour
{
    float damageDelay;

    private void Start()
    {
        Invoke("SelfDestroy", 4.1f);
    }
    private void OnTriggerStay(Collider other)
    {
        if (damageDelay >= 0)
            damageDelay -= Time.deltaTime;

        if (other.tag == "Enemy")
        {
            if (damageDelay <=0)
            {
                if (other.gameObject.TryGetComponent(out MonsterAI monster))
                    monster.MonsterDmged(1);

                damageDelay = 0.5f;
            }
        }
    }
    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
