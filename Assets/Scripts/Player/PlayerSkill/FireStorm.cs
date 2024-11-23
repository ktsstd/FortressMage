using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStorm : MonoBehaviour
{
    float damageDelay;
    private void Start()
    {
        Invoke("SelfDestroy", 5f);
    }

    private void Update()
    {
        if (damageDelay >= 0)
            damageDelay -= Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if (damageDelay <= 0)
        {
            if (other.gameObject.TryGetComponent(out MonsterAI monster))
                monster.MonsterDmged(3);

            damageDelay = 0.5f;
        }
    }

    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
