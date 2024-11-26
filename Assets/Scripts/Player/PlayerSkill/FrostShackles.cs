using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostShackles : MonoBehaviour
{
    float damageDelay = 0.5f;
    GameObject childObject;
    int count = 0;

    void Start()
    {
        childObject = transform.GetChild(0).gameObject;
        Invoke("SelfDestroy", 4f);
    }

    private void Update()
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
                    monster.MonsterDmged(1);
                    if (count == 6)
                        Debug.Log("�󸮱�");
                }
                Debug.Log("����������");
                if (count == 5)
                    Debug.Log("�󸮱�");
                damageDelay = 0.5f;
                count++;
            }
        }
    }
    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
