using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill4Dmg : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            PlayerController playerScript = other.GetComponent<PlayerController>();
            playerScript.OnHitPlayer(60f);
        }
    }
}
