using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill5dmged : MonoBehaviour
{
    void Start()
    {
        Invoke("Destroythis", 0.4f);
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            PlayerController playerScript = other.GetComponent<PlayerController>();
            playerScript.OnHitPlayer(100f);
        }
    }

    void Destroythis()
    {
        Destroy(gameObject);
    }
}
