using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricShot : MonoBehaviour
{
    public int damage;

    void Start()
    {
        Invoke("SelfDestroy", 0.5f);
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
    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
