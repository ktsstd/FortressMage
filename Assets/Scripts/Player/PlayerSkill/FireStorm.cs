using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStorm : MonoBehaviour
{
    Vector3 oriScale;
    float timeElapsed;
    float damageDelay;

    private void Start()
    {
        oriScale = transform.localScale;
        Invoke("SelfDestroy", 5f);
    }

    private void Update()
    {
        if (damageDelay >= 0)
            damageDelay -= Time.deltaTime;

        timeElapsed += Time.deltaTime;
        float lerpFactor = timeElapsed / 2f;
        transform.GetChild(0).localScale = Vector3.Lerp(oriScale * 0.1f, oriScale * 1.5f, lerpFactor);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (damageDelay <= 0)
            {
                if (other.gameObject.TryGetComponent(out MonsterAI monster))
                    monster.MonsterDmged(3);

                Debug.Log("¾ÆÀÌ¶ß°Å¿ö");

                damageDelay = 0.5f;
            }
        }
            
    }

    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
