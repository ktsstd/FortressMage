using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject[] characterPrefabs;
    public Transform[] spawnPoint;
    public TMP_Text NickText;

    public Transform[] monsterSpawnPoint;
    public Transform BossSpawnPoint;

    // public GameObject[] normalMonster;
    // public GameObject[] eliteMonster;
    // public GameObject bossMonster;
    public GameObject TestSpawnObj;
    public GameObject DEMObossObj;

    private int[] maxMonsterCount = { 5, 5, 5, 6, 7 };
    private float Wave = 1;

    private bool isStartWave = false;

    public int playerLv = 1;
    
    private Dictionary<int, bool> isTurretDestroyedAtWave = new Dictionary<int, bool>();

    private static GameManager _instance;

    void Start()
    {
        // PhotonNetwork.LocalPlayer에서 선택된 캐릭터 정보 가져오기
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("selectedCharacter"))
        {
            int selectedCharacterIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties["selectedCharacter"];

            // 스폰할 포인트를 배열에서 랜덤으로 선택 (혹은 인덱스에 따라 선택 가능)
            Transform selectedSpawnPoint = spawnPoint[Random.Range(0, spawnPoint.Length)];

            // 캐릭터를 선택된 스폰 포인트에서 스폰
            SpawnCharacter(selectedCharacterIndex, selectedSpawnPoint.position);
        }

        if (NickText != null)
        {
            NickText.text = PhotonNetwork.LocalPlayer.NickName;
        }
    }

    void SpawnCharacter(int characterIndex, Vector3 spawnPosition)
    {
        if (characterIndex >= 0 && characterIndex < characterPrefabs.Length)
        {
            // 선택된 캐릭터의 이름을 사용하여 PhotonNetwork.Instantiate
            string prefabName = "Player/" + characterPrefabs[characterIndex].name;

            // 캐릭터를 PhotonNetwork.Instantiate로 선택된 위치에 소환
            PhotonNetwork.Instantiate(prefabName, spawnPosition, Quaternion.identity);
        }
    }

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

    public string GetPlayerLvToString()
    {
        return "LV. " + playerLv;
    }

    public void StartWave()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Spawning with MasterClient");
            // StartCoroutine(StartSpawnMonster());
            StartCoroutine(StartTestWave());
            isStartWave = true;
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
            if (!isStartWave) return;

            PlayerController[] players = FindObjectsOfType<PlayerController>();
            for (int i = 0;i < players.Length; i++)
            {
                if (players[i].isDie)
                {
                    players[i].pv.RPC("StandUp", RpcTarget.All, null);
                }

                players[i].pv.RPC("PlayerLvUp", RpcTarget.All, null);
            }



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
        // { "Monster/Spirit of Fire", "Monster/MonsterI", "Monster/Spirit of Light", "Monster/Spirit of Dark",
        // "Monster/Spirit of Wind", "Monster/EliteMonster1", "Monster/EliteMonsetr2" }
        yield return new WaitForSeconds(15f);

        if (isTurretDestroyedAtWave.ContainsKey((int)Wave - 2) && isTurretDestroyedAtWave[(int)Wave - 2])
        {
            ResetTurretHealthOnWave(Wave);
            isTurretDestroyedAtWave[(int)Wave - 2] = false;
        }

        if (Wave == 1)
        {
            StartCoroutine(SpawnMonsters("Monster/Spirit of Fire", 11, 1f));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Light", 6, 1f));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Wind", 1, 0f));
        }
        else if (Wave == 2)
        {
            StartCoroutine(SpawnMonsters("Monster/Spirit of Fire", 16, 1f));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Light", 5, 1f));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Wind", 3, 1f));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Dark", 5, 1f));
        }
        else if (Wave == 3)
        {
            //PhotonNetwork.Instantiate("Boss/Boss", BossSpawnPoint.position, Quaternion.identity);
            DEMObossObj.SetActive(true);    
        }

        Wave += 1;
        Debug.Log(Wave);
    }

    private IEnumerator SpawnMonsters(string monsterName, int count, float spawnDelay)
    {
        for (int i = 0; i < count; i++)
        {
            int spawnPointRandom = Random.Range(0, monsterSpawnPoint.Length);
            PhotonNetwork.Instantiate(monsterName, monsterSpawnPoint[spawnPointRandom].position, Quaternion.identity);
            if (spawnDelay > 0f)
            {
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }

    private void ResetTurretHealthOnWave(float wave)
    {
        Turret turret = FindObjectOfType<Turret>();
        if (turret != null && turret.health == 0)
        {
            turret.ResetHealth();
            Debug.Log($"Turret health reset on Wave {wave}");
        }

        Skilltower skilltower = FindObjectOfType<Skilltower>();
        if (skilltower != null && skilltower.health == 0)
        {
            skilltower.ResetHealth();
            Debug.Log($"Skilltower health reset on Wave {wave}");
        }
    }

   public void CheckTurretDestroyedOnWave(float wave)
    {
        Turret turret = FindObjectOfType<Turret>();
        if (turret != null && turret.health == 0)
        {
            if (!isTurretDestroyedAtWave.ContainsKey((int)wave))
            {
                isTurretDestroyedAtWave[(int)wave] = true; // 웨이브에서 포탑 파괴 기록
                Debug.Log($"Turret destroyed at Wave {wave}");
            }
        }

        Skilltower skilltower = FindObjectOfType<Skilltower>();
        if (skilltower != null && skilltower.health == 0)
        {
            if (!isTurretDestroyedAtWave.ContainsKey((int)wave))
            {
                isTurretDestroyedAtWave[(int)wave] = true; // 웨이브에서 스킬타워 파괴 기록
                Debug.Log($"Skilltower destroyed at Wave {wave}");
            }
        }
    }

    public void TestButtonOn()
    {
        TestSpawnObj.SetActive(true);
    }

    public void TestButtonOff()
    {
        TestSpawnObj.SetActive(false);
    }

    public void TestSpawnFire()
    {
        int spawnPointRandom = Random.Range(0, monsterSpawnPoint.Length);
        string FireMonster = "Monster/Spirit of Fire";

        PhotonNetwork.Instantiate(FireMonster, monsterSpawnPoint[spawnPointRandom].position, Quaternion.identity);
    }

    public void TestSpawnIce()
    {
        int spawnPointRandom = Random.Range(0, monsterSpawnPoint.Length);
        string IceMonster = "Monster/MonsterI";

        PhotonNetwork.Instantiate(IceMonster, monsterSpawnPoint[spawnPointRandom].position, Quaternion.identity);
    }

    public void TestSpawnLight()
    {
        int spawnPointRandom = Random.Range(0, monsterSpawnPoint.Length);
        string LightMonster = "Monster/Spirit of Light";

        PhotonNetwork.Instantiate(LightMonster, monsterSpawnPoint[spawnPointRandom].position, Quaternion.identity);
    }

    public void TestSpawnDark()
    {
        int spawnPointRandom = Random.Range(0, monsterSpawnPoint.Length);
        string DarkMonster = "Monster/Spirit of Dark";

        PhotonNetwork.Instantiate(DarkMonster, monsterSpawnPoint[spawnPointRandom].position, Quaternion.identity);
    }

    public void TestSpawnWind()
    {
        int spawnPointRandom = Random.Range(0, monsterSpawnPoint.Length);
        string WindMonster = "Monster/Spirit of Wind";

        PhotonNetwork.Instantiate(WindMonster, monsterSpawnPoint[spawnPointRandom].position, Quaternion.identity);
    }

    public void TestSpawnElite1()
    {
        string BossMonster = "EliteMonster/Dragon";

        PhotonNetwork.Instantiate(BossMonster, BossSpawnPoint.position, Quaternion.identity);
    }

    public void TestSpawnBoss()
    {
        string BossMonster = "Boss/Boss";

        PhotonNetwork.Instantiate(BossMonster, BossSpawnPoint.position, Quaternion.identity);
    }

    public void StartScene()
    {
        SceneManager.LoadScene("New Scene");
    }

}