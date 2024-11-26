using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public Transform[] monsterSpawnPoint;

    // public GameObject[] normalMonster;
    // public GameObject[] eliteMonster;
    // public GameObject bossMonster;

    private int maxMonsterCount = 2;
    private float Wave = 1;

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Spawning with MasterClient");
            // StartCoroutine(StartSpawnMonster());
            StartCoroutine(StartTestWave());
        }
        else
        {
            Debug.Log("you are not MasterClient");
        }
    }

    public void CheckMonsters()
    {
        if (!isMonsterAlive())
        {
            StartCoroutine(StartTestWave());
        }
    }

    private bool isMonsterAlive()
    {
        MonsterAI[] monsters = FindObjectsOfType<MonsterAI>(); // 모든 MonsterAI 오브젝트 찾기
        int totalmonsterCount = monsters.Length;
        if (totalmonsterCount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator StartTestWave()
    {
        yield return new WaitForSeconds(10f);
        string[] FirstWaves = { "Monster/MonsterF", "Monster/MonsterI", "Monster/Spirit of Light" };
        string[] SecondWaves = { "Monster/Spirit of Dark", "Monster/Spirit of Wind", "Monster/EliteMonster1", "Monster/EliteMonsetr2" };

        if (Wave == 1)
        {
            for (int i = 0; i < maxMonsterCount; i++)
            {
                foreach (var FirstWave in FirstWaves)
                {
                    int spawnPointRandom = Random.Range(0, monsterSpawnPoint.Length);
                    PhotonNetwork.Instantiate(FirstWave, monsterSpawnPoint[spawnPointRandom].position, Quaternion.identity);
                    yield return new WaitForSeconds(FirstWave == "Monster/Spirit of Light" ? 2f : 0.3f);
                }
            }
        }
        else if (Wave == 2)
        {
            for (int i = 0; i < maxMonsterCount; i++)
            {
                foreach (var SecondWave in SecondWaves)
                {
                    int spawnPointRandom = Random.Range(0, monsterSpawnPoint.Length);
                    PhotonNetwork.Instantiate(SecondWave, monsterSpawnPoint[spawnPointRandom].position, Quaternion.identity);
                    yield return new WaitForSeconds(SecondWave == "Monster/EliteMonsetr2" ? 2f : 0.1f);
                }
            }
        }
        Wave += 1;
        Debug.Log(Wave);
    }
}