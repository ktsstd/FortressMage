using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostShackles : MonoBehaviour
{
    float damageDelay = 0f;
    GameObject childObject;
    int count = 0;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;

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
                    monster.OnMonsterSpeedDown(1f, 2f);
                    if (count == 5)
                        monster.OnMonsterStun(3f);
                }
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
