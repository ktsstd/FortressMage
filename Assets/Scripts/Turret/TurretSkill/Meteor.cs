using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    void Start()
    {
        Invoke("ColliderOn", 2f);
        Invoke("SelfDestroy", 3.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (other.gameObject.TryGetComponent(out MonsterAI monster))
            {
                monster.MonsterDmged(60);
                monster.OnMonsterStun(3f);
                monster.OnMonsterBurned(3, 5);
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
