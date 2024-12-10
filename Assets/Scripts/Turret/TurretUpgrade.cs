using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurretUpgradeManager : MonoBehaviourPunCallbacks
{
    // 공통 변수
    public GoldManager goldManager;  // 골드 매니저 참조

    public Button[] upgradeWindowButtons = new Button[6];

    public Button button;

    public Turret[] turrets = new Turret[2];  // 첫 번째, 두 번째 포탑을 배열로 관리
    public Button[] increaseFireRateButtons = new Button[2];  // 공격 속도 업그레이드 버튼
    public Button[] increaseDamageButtons = new Button[2];  // 데미지 업그레이드 버튼
    public Button[] upgradeRadiusButtons = new Button[2];  // 범위 업그레이드 버튼

    // 업그레이드 비용 및 제한 관련 변수
    public int turretUpgradeCost = 50;  // 포탑 업그레이드 비용
    public int turretMaxUpgrades = 5;   // 포탑 최대 업그레이드 횟수
    public float turretCostIncreaseRate = 1.5f;  // 포탑 업그레이드 비용 증가율
    private int[] turretCurrentUpgrades = { 0, 0 };  // 각 포탑의 업그레이드 횟수 (배열로 관리)

    public TMP_Text[] fireRateCostTexts = new TMP_Text[2];  // 공격 속도 업그레이드 비용 텍스트
    public TMP_Text[] damageCostTexts = new TMP_Text[2];     // 데미지 업그레이드 비용 텍스트
    public TMP_Text[] explosionRadiusCostTexts = new TMP_Text[2]; // 범위 업그레이드 비용 텍스트

    public TMP_Text[] fireRateUpgradeTexts = new TMP_Text[2];  // 공격 속도 업그레이드 횟수 텍스트
    public TMP_Text[] damageUpgradeTexts = new TMP_Text[2];     // 데미지 업그레이드 횟수 텍스트
    public TMP_Text[] explosionRadiusUpgradeTexts = new TMP_Text[2]; // 범위 업그레이드 횟수 텍스트

    public int damageUpgradeCost = 50;  // 데미지 업그레이드 비용
    public int damageMaxUpgrades = 10;  // 데미지 최대 업그레이드 횟수
    public float damageCostIncreaseRate = 1.2f;  // 데미지 업그레이드 비용 증가율
    private int[] damageCurrentUpgrades = { 0, 0 };  // 각 포탑의 데미지 업그레이드 횟수

    public float explosionRadiusUpgradeAmount = 1f;  // 범위 업그레이드 크기
    public int explosionRadiusUpgradeCost = 50;  // 범위 업그레이드 비용
    public int explosionRadiusMaxUpgrades = 5;  // 범위 업그레이드 최대 횟수
    private int[] explosionRadiusCurrentUpgrades = { 0, 0 };  // 각 포탑의 범위 업그레이드 횟수

    private void Start()
    {
        // 버튼 클릭 이벤트에 대한 공통 리스너 설정
        for (int i = 0; i < 2; i++)
        {
            int turretIndex = i;
            if (increaseFireRateButtons[i] != null)
                increaseFireRateButtons[i].onClick.AddListener(() => IncreaseStat(turretIndex, "fireRate"));

            if (increaseDamageButtons[i] != null)
                increaseDamageButtons[i].onClick.AddListener(() => IncreaseStat(turretIndex, "damage"));

            if (upgradeRadiusButtons[i] != null)
                upgradeRadiusButtons[i].onClick.AddListener(() => IncreaseStat(turretIndex, "explosionRadius"));
        }

        if (PhotonNetwork.IsMasterClient)
        {
            button.gameObject.SetActive(true); // 방장이면 버튼을 활성화
        }
        else
        {
            button.gameObject.SetActive(false); // 방장이 아니면 버튼을 비활성화
        }

        UpdateUpgradeUI();
    }

    // 포탑 업그레이드 로직
    public void IncreaseStat(int turretIndex, string statType)
    {
        Turret turret = turrets[turretIndex];

        // 각 업그레이드 조건 및 비용
        if (statType == "fireRate" && turretCurrentUpgrades[turretIndex] < turretMaxUpgrades)
        {
            UpgradeFireRate(turret, turretIndex);
        }
        else if (statType == "damage" && damageCurrentUpgrades[turretIndex] < damageMaxUpgrades)
        {
            UpgradeDamage(turret, turretIndex);
        }
        else if (statType == "explosionRadius" && explosionRadiusCurrentUpgrades[turretIndex] < explosionRadiusMaxUpgrades)
        {
            UpgradeExplosionRadius(turret, turretIndex);
        }
    }

    // 공격 속도 업그레이드 함수
    private void UpgradeFireRate(Turret turret, int turretIndex)
    {
        if (goldManager.SpendGold(turretUpgradeCost))
        {
            turret.fireRate -= turret.fireRateIncreaseAmount;
            if (turret.fireRate < turret.maxFireRate) turret.fireRate = turret.maxFireRate;

            turretCurrentUpgrades[turretIndex]++;
            turretUpgradeCost = Mathf.RoundToInt(turretUpgradeCost * turretCostIncreaseRate);
            photonView.RPC("SyncStat", RpcTarget.OthersBuffered, turretIndex, "fireRate", turret.fireRate);
            photonView.RPC("SyncUpgradeStat", RpcTarget.OthersBuffered, turretIndex, "fireRate", turretCurrentUpgrades[turretIndex]);
            photonView.RPC("SyncUpgradeCost", RpcTarget.OthersBuffered, turretIndex, "fireRate", turretUpgradeCost);
            UpdateUpgradeUI();  // 업그레이드 UI 업데이트

            DisableUpgradeButtonWindow(turretIndex, "fireRate");
        }
    }

    // 데미지 업그레이드 함수
    private void UpgradeDamage(Turret turret, int turretIndex)
    {
        if (goldManager.SpendGold(damageUpgradeCost))
        {
            turret.damage += turret.increaseAmount;
            damageCurrentUpgrades[turretIndex]++;
            damageUpgradeCost = Mathf.RoundToInt(damageUpgradeCost * damageCostIncreaseRate);
            photonView.RPC("SyncStat", RpcTarget.OthersBuffered, turretIndex, "damage", turret.damage);
            photonView.RPC("SyncUpgradeStat", RpcTarget.OthersBuffered, turretIndex, "damage", damageCurrentUpgrades[turretIndex]);
            photonView.RPC("SyncUpgradeCost", RpcTarget.OthersBuffered, turretIndex, "damage", damageUpgradeCost);
            UpdateUpgradeUI();  // 업그레이드 UI 업데이트

            DisableUpgradeButtonWindow(turretIndex, "damage");
        }
    }

    // 폭발 범위 업그레이드 함수
    private void UpgradeExplosionRadius(Turret turret, int turretIndex)
    {
        if (goldManager.SpendGold(explosionRadiusUpgradeCost))
        {
            turret.explosionRadius += explosionRadiusUpgradeAmount;
            explosionRadiusCurrentUpgrades[turretIndex]++;
            explosionRadiusUpgradeCost = Mathf.RoundToInt(explosionRadiusUpgradeCost * 1.5f);
            photonView.RPC("SyncStat", RpcTarget.OthersBuffered, turretIndex, "explosionRadius", turret.explosionRadius);
            photonView.RPC("SyncUpgradeStat", RpcTarget.OthersBuffered, turretIndex, "explosionRadius", explosionRadiusCurrentUpgrades[turretIndex]);
            photonView.RPC("SyncUpgradeCost", RpcTarget.OthersBuffered, turretIndex, "explosionRadius", explosionRadiusUpgradeCost);
            UpdateUpgradeUI();  // 업그레이드 UI 업데이트

            DisableUpgradeButtonWindow(turretIndex, "explosionRadius");
        }
    }

    private void DisableUpgradeButtonWindow(int turretIndex, string upgradeType)
    {
        // 업그레이드 타입에 따라 적절한 업그레이드 버튼 창을 여는 버튼을 비활성화
        if (turretIndex == 0) // 첫 번째 타워
        {
            if (upgradeType == "fireRate" && turretCurrentUpgrades[turretIndex] >= turretMaxUpgrades)
                upgradeWindowButtons[0].interactable = false;
            else if (upgradeType == "damage" && damageCurrentUpgrades[turretIndex] >= damageMaxUpgrades)
                upgradeWindowButtons[1].interactable = false;
            else if (upgradeType == "explosionRadius" && explosionRadiusCurrentUpgrades[turretIndex] >= explosionRadiusMaxUpgrades)
                upgradeWindowButtons[2].interactable = false;
        }
        else if (turretIndex == 1) // 두 번째 타워
        {
            if (upgradeType == "fireRate" && turretCurrentUpgrades[turretIndex] >= turretMaxUpgrades)
                upgradeWindowButtons[3].interactable = false;
            else if (upgradeType == "damage" && damageCurrentUpgrades[turretIndex] >= damageMaxUpgrades)
                upgradeWindowButtons[4].interactable = false;
            else if (upgradeType == "explosionRadius" && explosionRadiusCurrentUpgrades[turretIndex] >= explosionRadiusMaxUpgrades)
                upgradeWindowButtons[5].interactable = false;
        }
    }

    private void UpdateUpgradeUI()
    {
        for (int i = 0; i < 2; i++)
        {
            // 업그레이드 횟수 텍스트 업데이트
            if (fireRateUpgradeTexts[i] != null)
                fireRateUpgradeTexts[i].text = "" + turretCurrentUpgrades[i] + "/" + turretMaxUpgrades;

            if (damageUpgradeTexts[i] != null)
                damageUpgradeTexts[i].text = "" + damageCurrentUpgrades[i] + "/" + damageMaxUpgrades;

            if (explosionRadiusUpgradeTexts[i] != null)
                explosionRadiusUpgradeTexts[i].text = "" + explosionRadiusCurrentUpgrades[i] + "/" + explosionRadiusMaxUpgrades;

            // 강화 비용 텍스트 업데이트
            if (fireRateCostTexts[i] != null)
                fireRateCostTexts[i].text = "강화 비용: " + turretUpgradeCost;
            if (damageCostTexts[i] != null)
                damageCostTexts[i].text = "강화 비용: " + damageUpgradeCost;
            if (explosionRadiusCostTexts[i] != null)
                explosionRadiusCostTexts[i].text = "강화 비용: " + explosionRadiusUpgradeCost;
        }
    }

    // RPC로 모든 클라이언트에 업그레이드 상태 동기화
    [PunRPC]
    public void SyncStat(int turretIndex, string statType, float newValue)
    {
        Turret turret = turrets[turretIndex];

        if (statType == "fireRate")
        {
            turret.fireRate = newValue;
        }
        else if (statType == "damage")
        {
            turret.damage = newValue;
        }
        else if (statType == "explosionRadius")
        {
            turret.explosionRadius = newValue;
        }
    }

    [PunRPC]
    public void SyncUpgradeStat(int turretIndex, string statType, int upgradeCount)
    {
        if (statType == "fireRate")
        {
            turretCurrentUpgrades[turretIndex] = upgradeCount;
        }
        else if (statType == "damage")
        {
            damageCurrentUpgrades[turretIndex] = upgradeCount;
        }
        else if (statType == "explosionRadius")
        {
            explosionRadiusCurrentUpgrades[turretIndex] = upgradeCount;
        }

        UpdateUpgradeUI();  // UI 동기화
    }

    [PunRPC]
    public void SyncUpgradeCost(int turretIndex, string statType, int newCost)
    {
        if (statType == "fireRate")
        {
            turretUpgradeCost = newCost;
        }
        else if (statType == "damage")
        {
            damageUpgradeCost = newCost;
        }
        else if (statType == "explosionRadius")
        {
            explosionRadiusUpgradeCost = newCost;
        }

        UpdateUpgradeUI();  // UI 동기화
    }
}