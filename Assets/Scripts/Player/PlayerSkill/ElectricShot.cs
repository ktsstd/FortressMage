using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricShot : MonoBehaviour
{
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
                monster.MonsterDmged(10);
            }
            Debug.Log("¹ø°³");
        }
    }
    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
