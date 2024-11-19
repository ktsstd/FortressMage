using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviourPun
{
    public Transform[] monsterSpawnPoint;

    public GameObject[] normalMonster;
    public GameObject[] eliteMonster;
    public GameObject bossMonster;

    private int maxMonsterCount = 10;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Spawning with MasterClient");
            StartCoroutine(StartSpawnMonster());
        }
        else
        {
            Debug.Log("you are not MasterClient");
        }
    }

    private IEnumerator StartSpawnMonster()
    {
        for (int i = 0; i < maxMonsterCount; i++)
        {
            int normalMonsterRandom = Random.Range(0, normalMonster.Length);
            int spawnPointRandom = Random.Range(0, monsterSpawnPoint.Length);

            GameObject monster = PhotonNetwork.Instantiate(normalMonster[normalMonsterRandom].name, monsterSpawnPoint[spawnPointRandom].position, Quaternion.identity);
            yield return new WaitForSeconds(5f);
        }
    }
}