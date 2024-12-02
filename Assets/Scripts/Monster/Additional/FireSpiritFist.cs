using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpiritFist : MonoBehaviour
{
    void OnColliderEnter(Collider other)
    {
        FireSpirit firescript = transform.parent.GetComponent<FireSpirit>();
        if (firescript.StartAtking)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController playerScript = other.GetComponent<PlayerController>();
                playerScript.OnHitPlayer(firescript.MonsterDmg);
            }

            if (other.CompareTag("skilltower"))
            {
                Skilltower skillTowerScript = other.GetComponent<Skilltower>(); 
                skillTowerScript.TakeDamage(firescript.MonsterDmg);
            }
            
            if (other.CompareTag("Castle"))
            {
                Wall castleScript = other.GetComponent<Wall>();
                castleScript.TakeDamage(firescript.MonsterDmg);
            }

            if (other.CompareTag("turret"))
            {
                Turret towerScript = other.GetComponent<Turret>();
                towerScript.TakeDamage(firescript.MonsterDmg);
            }
            firescript.StartAtking = false;
        }
    }
}
