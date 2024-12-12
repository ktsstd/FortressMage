using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public GameObject fireRoad;
    public Vector3 targetPos;
    public int damage;

    float summonDelay = 0.2f;
    bool isExplosion = false;
    bool isDestroy = false;

    private AudioSource audioSource;
    public AudioClip[] audioClip;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;
        audioSource.PlayOneShot(audioClip[0], 0.5f);
    }

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
                    summonDelay = 0.2f;
                }
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(true);
                Invoke("SelfDestroy", 1.5f);
                audioSource.PlayOneShot(audioClip[1], 0.5f);
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
                    monster.MonsterDmged(damage);
                    monster.OnMonsterBurned(5, 1);
                }
            }
        }
    }

    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
