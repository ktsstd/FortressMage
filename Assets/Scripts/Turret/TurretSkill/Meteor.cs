using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] audioClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;

        Invoke("ColliderOn", 2f);
        Invoke("SelfDestroy", 3.5f);

        audioSource.PlayOneShot(audioClip[0], 0.2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (other.gameObject.TryGetComponent(out MonsterAI monster))
            {
                monster.MonsterDmged(60);
                monster.OnMonsterStun(3f);
                monster.OnMonsterBurned(3, 5);
            }
        }
    }

    void ColliderOn()
    {
        GetComponent<SphereCollider>().enabled = true;
        audioSource.PlayOneShot(audioClip[1], 0.3f);
    }

    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
