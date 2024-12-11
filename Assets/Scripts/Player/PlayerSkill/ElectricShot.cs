using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricShot : MonoBehaviour
{
    public int damage;

    private AudioSource audioSource;
    public AudioClip audioClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;

        Invoke("SelfDestroy", 0.5f);

        audioSource.PlayOneShot(audioClip, 0.3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (other.gameObject.TryGetComponent(out MonsterAI monster))
            {
                monster.MonsterDmged(damage);
            }
        }
    }
    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
