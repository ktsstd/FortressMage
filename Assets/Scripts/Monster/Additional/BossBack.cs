using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBack : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerScirpt = other.GetComponent<PlayerController>();
            playerScirpt.OnPlayerKnockBack(transform);
        }
    }
}
