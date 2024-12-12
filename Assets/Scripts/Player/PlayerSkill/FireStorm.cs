using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStorm : MonoBehaviour
{
    public int damage;

    Vector3 oriScale;
    float timeElapsed;
    float damageDelay;

    private AudioSource audioSource;
    public AudioClip audioClip;

    private void Start()
    {
        oriScale = transform.localScale;
        Invoke("SelfDestroy", 5f);

        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;
        audioSource.PlayOneShot(audioClip, 0.5f);
    }

    private void Update()
    {
        if (damageDelay >= 0)
            damageDelay -= Time.deltaTime;

        timeElapsed += Time.deltaTime;
        float lerpFactor = timeElapsed / 2f;
        transform.GetChild(0).localScale = Vector3.Lerp(oriScale * 0.5f, oriScale * 1.5f, lerpFactor);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (damageDelay <= 0)
            {
                if (other.gameObject.TryGetComponent(out MonsterAI monster))
                {
                    monster.MonsterDmged(damage);
                }

                Debug.Log(damage);

                damageDelay = 0.5f;
            }
        }   
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (other.gameObject.TryGetComponent(out MonsterAI monster))
            {
                monster.OnMonsterBurningStart(3, 2);
            }
        }

    }

    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
