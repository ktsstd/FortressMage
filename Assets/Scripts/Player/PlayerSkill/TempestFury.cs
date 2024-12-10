using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempestFury : MonoBehaviour
{
    public int damage;

    void Start()
    {
        Invoke("ColliderOn", 0.5f);
        Invoke("SelfDestroy", 1.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (other.gameObject.TryGetComponent(out MonsterAI monster))
            {
                monster.MonsterDmged(damage);
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
