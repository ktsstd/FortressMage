using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Transform[] monsterSpawnPoint;

    public GameObject[] normalMonster;
    public GameObject[] eliteMonster;
    public GameObject bossMonster;

    private int maxMonsterCount = 10;

    void Start()
    {
        StartCoroutine(StartSpawnMonster());
    }

    private IEnumerator StartSpawnMonster()
    {
        for (int i = 0; i < maxMonsterCount; i++)
        {
            int normalMonsterRandom = Random.Range(0, normalMonster.Length);
            int spawnPointRandom = Random.Range(0, monsterSpawnPoint.Length);

            Instantiate(normalMonster[normalMonsterRandom], monsterSpawnPoint[spawnPointRandom].position, Quaternion.identity);
            yield return new WaitForSeconds(5f);
        }
    }
}