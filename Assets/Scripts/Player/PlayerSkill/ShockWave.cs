using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    public int damage;

    void Start()
    {
        Invoke("ColliderOn", 0.5f);
        Invoke("SelfDestroy", 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (other.gameObject.TryGetComponent(out MonsterAI monster))
            {
                monster.MonsterDmged(damage);
                monster.OnMonsterStun(0.5f);
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
