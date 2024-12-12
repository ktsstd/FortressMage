using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject[] characterPrefabs;
    public Transform[] spawnPoint;
    public TMP_Text WaveText; 
    public TMP_Text NickText;

    public Transform[] monsterSpawnPoint;
    public Transform BossSpawnPoint;

    // public GameObject[] normalMonster;
    // public GameObject[] eliteMonster;
    // public GameObject bossMonster;
    public GameObject TestSpawnObj;
    public GameObject DEMObossObj;
    public GameObject WaveTextImg;

    public Image[] EndImage;

    private int[] maxMonsterCount = { 5, 5, 5, 6, 7 };
    private int totalmonsterCount;
    private float Wave = 0;

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
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Spawning with MasterClient");
            // StartCoroutine(StartSpawnMonster());
            photonView.RPC("StartWaveTimer", RpcTarget.All);
        }
        if (NickText != null)
        {
            NickText.text = PhotonNetwork.LocalPlayer.NickName;
        }
    }

    [PunRPC]
    void StartWaveTimer()
    {
        StartCoroutine(WaveTimer());
    }

    void Update()
    {
        if (isStartWave)
        {
            MonsterAI[] monsters = FindObjectsOfType<MonsterAI>(); 
            totalmonsterCount = monsters.Length;
            WaveText.text = "Wave: " + Wave.ToString() + "\nCurMonster: " + totalmonsterCount;
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
            StartCoroutine(WaveTimer());
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
                players[i].pv.RPC("PlayerRecovery", RpcTarget.All, null);
                players[i].pv.RPC("PlayerLvUp", RpcTarget.All, null);
            }

            Turret[] turret = FindObjectsOfType<Turret>();
            for (int i = 0; i < turret.Length; i++)
            {
                if (turret[i].canAttack == false)
                {
                    turret[i].photonView.RPC("ResetHealth", RpcTarget.All, null);
                }
            }
            isStartWave = false;
            photonView.RPC("StartWaveTimer", RpcTarget.All);
        }
    }

    private bool isMonsterAlive()
    {
        MonsterAI[] monsters = FindObjectsOfType<MonsterAI>(); // 모든 MonsterAI 오브젝트 찾기
        totalmonsterCount = monsters.Length;
        if (totalmonsterCount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator WaveTimer()
    {
        Wave += 1;
        int remainTimer = 15;
        if (Wave == 9)
        {
            // 승리
        }
        for (int remainTimer = 15; remainTimer > 0; remainTimer --)
        {
            WaveText.text = "next: Wave " + Wave.ToString() + "\nTime: " + remainTimer + "seconds";
            yield return new WaitForSeconds(1f);
        }
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(StartTestWave());
        }
        isStartWave = true;
    }

    private IEnumerator StartTestWave()
    {
        // { "Monster/Spirit of Fire", "Monster/MonsterI", "Monster/Spirit of Light", "Monster/Spirit of Dark",
        // "Monster/Spirit of Wind", "Monster/EliteMonster1", "Monster/EliteMonsetr2" }

        if (isTurretDestroyedAtWave.ContainsKey((int)Wave - 2) && isTurretDestroyedAtWave[(int)Wave - 2])
        {
            ResetTurretHealthOnWave(Wave);
            isTurretDestroyedAtWave[(int)Wave - 2] = false;
        }

        if (Wave == 1)
        {
            StartCoroutine(SpawnMonsters("Monster/Spirit of Fire", 5, 1));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Ice", 3, 1.5f));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Light", 2, 2f));            
        }
        else if (Wave == 2)
        {
            StartCoroutine(SpawnMonsters("Monster/Spirit of Fire", 7, 1));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Ice", 5, 1.5f));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Light", 3, 2f));  
            StartCoroutine(SpawnMonsters("Monster/Spirit of Dark", 2, 3f));
        }
        else if (Wave == 3)
        {
            StartCoroutine(SpawnMonsters("Monster/Spirit of Fire", 10, 1));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Ice", 8, 1.5f));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Light", 4, 2f));  
            StartCoroutine(SpawnMonsters("Monster/Spirit of Dark", 3, 3f));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Wind", 1, 2.5f));
        }
        else if (Wave == 4)
        {
            StartCoroutine(SpawnMonsters("Monster/Spirit of Fire", 12, 1));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Ice", 9, 1.5f));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Light", 5, 2f));  
            StartCoroutine(SpawnMonsters("Monster/Spirit of Dark", 3, 3f));
            StartCoroutine(SpawnMonsters("EliteMonster/Dragon", 1, 2f));
        }
        else if (Wave == 5)
        {
            StartCoroutine(SpawnMonsters("Monster/Spirit of Fire", 15, 1));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Ice", 11, 1.5f));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Light", 5, 2f));  
            StartCoroutine(SpawnMonsters("Monster/Spirit of Dark", 4, 3f));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Wind", 2, 3f));
            StartCoroutine(SpawnMonsters("EliteMonster/Dragon", 1, 2f));
        }
        else if (Wave == 6)
        {
            StartCoroutine(SpawnMonsters("Monster/Spirit of Fire", 17, 1));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Ice", 13, 1.5f));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Light", 5, 2f));  
            StartCoroutine(SpawnMonsters("Monster/Spirit of Dark", 4, 3f));
            StartCoroutine(SpawnMonsters("EliteMonster/EBs", 1, 2f));
        }
        else if (Wave == 7)
        {
            StartCoroutine(SpawnMonsters("Monster/Spirit of Fire", 20, 1));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Ice", 16, 1.5f));
            StartCoroutine(SpawnMonsters("Monster/Spirit of Light", 8, 2f));  
            StartCoroutine(SpawnMonsters("Monster/Spirit of Dark", 6, 3f));
            StartCoroutine(SpawnMonsters("EliteMonster/Dragon", 1, 2f));
            StartCoroutine(SpawnMonsters("EliteMonster/EBs", 1, 2f));
        }
        else if (Wave == 8)
        {
            PhotonNetwork.Instantiate("Boss/Boss", BossSpawnPoint.position, Quaternion.identity);
            // DEMObossObj.SetActive(true);    
        }
        yield break;
    }

    private IEnumerator SpawnMonsters(string monsterName, int count, float spawnDelay)
    {
        for (int i = 0; i < count; i++)
        {
            int spawnPointRandom = Random.Range(0, monsterSpawnPoint.Length);
            GameObject monster = PhotonNetwork.Instantiate(monsterName, monsterSpawnPoint[spawnPointRandom].position, Quaternion.identity);

            PhotonView photonView = monster.GetComponent<PhotonView>();

            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);

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

    // public override void OnMasterClientSwitched(Player newMasterClient)
    // {
    //     UpdatePlayerListUI(); // ���ο� ���� ���� ����
    // }

    public IEnumerator DefeatEvent()
    {
        EndImage[0].gameObject.SetActive(true);

        float timeElapsed = 0f;
        Color color = EndImage[0].color;

        while (timeElapsed < 1)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timeElapsed / 1);
            color.a = alpha;
            EndImage[0].color = color;
            yield return null;
        }

        yield return new WaitForSeconds(3);
        DefeatScene();
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
        string IceMonster = "Monster/Spirit of Ice";

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
        int spawnPointRandom = Random.Range(0, monsterSpawnPoint.Length);
        string EliteMonster1 = "EliteMonster/Dragon";

        PhotonNetwork.Instantiate(EliteMonster1, monsterSpawnPoint[spawnPointRandom].position, Quaternion.identity);
    }

    public void TestSpawnElite2()
    {
        int spawnPointRandom = Random.Range(0, monsterSpawnPoint.Length);
        string EliteMonster2 = "EliteMonster/EBs";

        PhotonNetwork.Instantiate(EliteMonster2, monsterSpawnPoint[spawnPointRandom].position, Quaternion.identity);
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


    public void VictoryScene()
    {
        SceneManager.LoadScene("VictoryScene");
    }

    public void DefeatScene()
    {
        SceneManager.LoadScene("DefeatScene");
    }
}