using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill2 : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerScript = other.GetComponent<PlayerController>();
            playerScript.OnPlayerStun(0.3f);
            playerScript.OnHitPlayer(10f);
        }        
    }
}
