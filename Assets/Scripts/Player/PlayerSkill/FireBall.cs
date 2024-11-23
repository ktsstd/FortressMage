using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public GameObject fireRoad;
    public Vector3 targetPos;
    public float damage;

    float summonDelay = 0;
    bool isExplosion = false;
    bool isDestroy = false;

    void Update()
    {
        if (targetPos != null && !isDestroy)
        {
            if (transform.position != targetPos && !isExplosion)
            {
                if (summonDelay >= 0)
                    summonDelay -= Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, 6f * Time.deltaTime);
                if (summonDelay <= 0 )
                {
                    GameObject fire = Instantiate(fireRoad, new Vector3(transform.position.x, 0f, transform.position.z), transform.rotation);
                    summonDelay = 0.1f;
                }
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
                    monster.MonsterDmged(15);
                }
            }
        }
    }

    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
