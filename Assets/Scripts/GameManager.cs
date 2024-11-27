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
    private bool isTurretDestroyedAtWave1 = false; // 웨이브 1에서 파괴된 포탑 추적
    private bool isTurretDestroyedAtWave2 = false; // 웨이브 2에서 파괴된 포탑 추적

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

    public float GetWave()
    {
        return Wave;
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
                foreach (var FirstWave in FirstWaves)
                {
                    int spawnPointRandom = Random.Range(0, monsterSpawnPoint.Length);
                    PhotonNetwork.Instantiate(FirstWave, monsterSpawnPoint[spawnPointRandom].position, Quaternion.identity);
                    yield return new WaitForSeconds(FirstWave == "Monster/Spirit of Light" ? 2f : 0.3f);
                }
            }
        }

        else if (Wave == 3)
        {
            if (Wave == 3 && isTurretDestroyedAtWave1) // 웨이브 1에서 파괴된 경우
            {
                ResetTurretHealthOnWave(3);
            }
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

        else if (Wave == 4)
        {
            if (Wave == 4 && isTurretDestroyedAtWave2) // 웨이브 2에서 파괴된 경우
            {
                ResetTurretHealthOnWave(4);
            }
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
        Wave += 1;
        Debug.Log(Wave);
    }

    private void ResetTurretHealthOnWave(float wave)
    {
        Turret turret = FindObjectOfType<Turret>(); // 유일한 Turret 객체 찾기
        if (turret != null && turret.health == 0)
        {
            turret.ResetHealth(); // 포탑 체력 리셋
            Debug.Log("Turret health reset on Wave " + wave);
        }

        Skilltower skilltower = FindObjectOfType<Skilltower>();
        if (skilltower != null && skilltower.health == 0)
        {
            skilltower.ResetHealth();
            Debug.Log("Skilltower health reset on Wave " + wave);
        }
    }

    public void CheckTurretDestroyedOnWave(float wave)
    {
        Turret turret = FindObjectOfType<Turret>(); // 유일한 Turret 객체 찾기
        if (turret != null && turret.health == 0)
        {
            if (wave == 1)
            {
                isTurretDestroyedAtWave1 = true; // 웨이브 1에서 포탑이 파괴되었음을 추적
            }
            else if (wave == 2)
            {
                isTurretDestroyedAtWave2 = true; // 웨이브 2에서 포탑이 파괴되었음을 추적
            }
        }
        Skilltower skilltower = FindObjectOfType<Skilltower>();
        if (skilltower != null && skilltower.health == 0)
        {
            if (wave == 1)
            {
                isTurretDestroyedAtWave1 = true; // 웨이브 1에서 포탑이 파괴되었음을 추적
            }
            else if (wave == 2)
            {
                isTurretDestroyedAtWave2 = true; // 웨이브 2에서 포탑이 파괴되었음을 추적
            }
        }
    }
}