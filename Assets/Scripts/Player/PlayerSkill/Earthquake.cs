using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Earthquake : MonoBehaviour
{
    public int damage;

    public GameObject[] stones;

    private AudioSource audioSource;
    public AudioClip[] audioClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;

        audioSource.PlayOneShot(audioClip[0], 0.3f);
        Invoke("SelfDestroy", 3f);
        StartCoroutine("StonesActiveOn");
    }
   
    IEnumerator StonesActiveOn()
    {
        stones[0].SetActive(true);
        audioSource.PlayOneShot(audioClip[1], 0.3f);
        yield return new WaitForSeconds(0.5f);
        stones[1].SetActive(true);
        audioSource.PlayOneShot(audioClip[1], 0.3f);
        yield return new WaitForSeconds(0.5f);
        stones[2].SetActive(true);
        audioSource.PlayOneShot(audioClip[1], 0.3f);
        yield return new WaitForSeconds(0.5f);
        stones[3].SetActive(true);
        audioSource.PlayOneShot(audioClip[1], 0.3f);
    }

    void Update()
    {
        for (int i = 0; i < stones.Length; i++)
        {
            if (stones[i].activeSelf)
            {
                stones[i].transform.position = Vector3.MoveTowards(stones[i].transform.position, transform.position, 5f * Time.deltaTime);
            }
            if (stones[i].transform.position == Vector3.zero)
            {
                stones[i].GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (other.gameObject.TryGetComponent(out MonsterAI monster))
            {
                monster.MonsterDmged(damage);
                monster.OnMonsterSpeedDown(1f, 2f);
            }
        }
    }
    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
