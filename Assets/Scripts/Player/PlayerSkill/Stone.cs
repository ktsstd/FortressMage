using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public Vector3 targetPos;
    public float damage;

    float summonDelay = 0.2f;
    bool isExplosion = false;
    bool isDestroy = false;
    Vector3 oriScale;
    float timeElapsed;

    private void Start()
    {
        oriScale = transform.localScale;
    }

    void Update()
    {
        if (targetPos != null && !isDestroy)
        {
            if (transform.position != targetPos && !isExplosion)
            {
                if (summonDelay >= 0)
                    summonDelay -= Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, 8f * Time.deltaTime);

                timeElapsed += Time.deltaTime;
                float lerpFactor = timeElapsed / 0.6f;
                transform.localScale = Vector3.Lerp(oriScale * 0.2f, oriScale, lerpFactor);
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(true);
                Invoke("SelfDestroy", 1.5f);
                isDestroy = true;
            }
        }
        else
            return;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (!isExplosion)
            {
                isDestroy = true;
                isExplosion = true;
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(true);
                Invoke("SelfDestroy", 1.5f);
            }
            else
            {
                if (other.gameObject.TryGetComponent(out MonsterAI monster))
                {
                    monster.MonsterDmged(20);
                }
            }
        }
    }

    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}