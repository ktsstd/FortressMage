using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IceBullet : MonoBehaviour
{
    public int damage;

    public Vector3 targetPos;

    bool isExplosion = false;
    bool isDestroy = false;
    bool isShoot = false;

    private AudioSource audioSource;
    public AudioClip[] audioClip;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;

        Invoke("ShootBullet", 1f);

        audioSource.PlayOneShot(audioClip[0], 0.2f);
    }

    void Update()
    {
        if (targetPos != null && !isDestroy)
        {
            if (transform.position != targetPos && !isExplosion)
            {
                if (isShoot)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, 20f * Time.deltaTime);
                    Vector3 direction = (targetPos - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(direction);

                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation * Quaternion.Euler(new Vector3(0, 180, 0)), 0.1f);
                }
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(true);
                Invoke("SelfDestroy", 1f);
                audioSource.PlayOneShot(audioClip[1], 0.2f);
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
                Invoke("SelfDestroy", 1f);
            }
            else
            {
                if (other.gameObject.TryGetComponent(out MonsterAI monster))
                {
                    monster.MonsterDmged(damage);
                }
            }
        }
    }
    void ShootBullet()
    {
        isShoot = true;
    }
    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
