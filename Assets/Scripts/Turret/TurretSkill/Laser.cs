using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip audioClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        Invoke("SoundOn", 2f);
        Invoke("ColliderOn", 2.5f);
        Invoke("SelfDestroy", 3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (other.gameObject.TryGetComponent(out MonsterAI monster))
            {
                monster.MonsterDmged(100);
            }
        }
    }

    void SoundOn()
    {
        audioSource.PlayOneShot(audioClip, 0.2f);
    }

    void ColliderOn()
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
