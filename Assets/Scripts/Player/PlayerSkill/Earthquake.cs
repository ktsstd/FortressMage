using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Earthquake : MonoBehaviour
{
    public GameObject[] stones;

    void Start()
    {
        Invoke("SelfDestroy", 3f);
        StartCoroutine("StonesActiveOn");
    }
   
    IEnumerator StonesActiveOn()
    {
        stones[0].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        stones[1].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        stones[2].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        stones[3].SetActive(true);
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
                monster.MonsterDmged(10);
            }
            Debug.Log("πŸ¿ß");
        }
    }
    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
