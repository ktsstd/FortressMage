using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRoad : MonoBehaviour
{
    private void Start()
    {
        Invoke("SelfDestroy", 4.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (other.gameObject.TryGetComponent(out MonsterAI monster))
                monster.MonsterDmged(1);
        }
    }

    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
