using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurretUpgradeManager : MonoBehaviourPunCallbacks
{
    // ���� ����
    public GoldManager goldManager;  // ��� �Ŵ��� ����

    public Button[] upgradeWindowButtons = new Button[6];

    public Button button;

    public Turret[] turrets = new Turret[2];  // ù ��°, �� ��° ��ž�� �迭�� ����
    public Button[] increaseFireRateButtons = new Button[2];  // ���� �ӵ� ���׷��̵� ��ư
    public Button[] increaseDamageButtons = new Button[2];  // ������ ���׷��̵� ��ư
    public Button[] upgradeRadiusButtons = new Button[2];  // ���� ���׷��̵� ��ư

    // ���׷��̵� ��� �� ���� ���� ����
    public int turretUpgradeCost = 50;  // ��ž ���׷��̵� ���
    public int turretMaxUpgrades = 5;   // ��ž �ִ� ���׷��̵� Ƚ��
    public float turretCostIncreaseRate = 1.5f;  // ��ž ���׷��̵� ��� ������
    private int[] turretCurrentUpgrades = { 0, 0 };  // �� ��ž�� ���׷��̵� Ƚ�� (�迭�� ����)

    public TMP_Text[] fireRateCostTexts = new TMP_Text[2];  // ���� �ӵ� ���׷��̵� ��� �ؽ�Ʈ
    public TMP_Text[] damageCostTexts = new TMP_Text[2];     // ������ ���׷��̵� ��� �ؽ�Ʈ
    public TMP_Text[] explosionRadiusCostTexts = new TMP_Text[2]; // ���� ���׷��̵� ��� �ؽ�Ʈ

    public TMP_Text[] fireRateUpgradeTexts = new TMP_Text[2];  // ���� �ӵ� ���׷��̵� Ƚ�� �ؽ�Ʈ
    public TMP_Text[] damageUpgradeTexts = new TMP_Text[2];     // ������ ���׷��̵� Ƚ�� �ؽ�Ʈ
    public TMP_Text[] explosionRadiusUpgradeTexts = new TMP_Text[2]; // ���� ���׷��̵� Ƚ�� �ؽ�Ʈ

    public int damageUpgradeCost = 50;  // ������ ���׷��̵� ���
    public int damageMaxUpgrades = 10;  // ������ �ִ� ���׷��̵� Ƚ��
    public float damageCostIncreaseRate = 1.2f;  // ������ ���׷��̵� ��� ������
    private int[] damageCurrentUpgrades = { 0, 0 };  // �� ��ž�� ������ ���׷��̵� Ƚ��

    public float explosionRadiusUpgradeAmount = 1f;  // ���� ���׷��̵� ũ��
    public int explosionRadiusUpgradeCost = 50;  // ���� ���׷��̵� ���
    public int explosionRadiusMaxUpgrades = 5;  // ���� ���׷��̵� �ִ� Ƚ��
    private int[] explosionRadiusCurrentUpgrades = { 0, 0 };  // �� ��ž�� ���� ���׷��̵� Ƚ��

    private void Start()
    {
        // ��ư Ŭ�� �̺�Ʈ�� ���� ���� ������ ����
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
            button.gameObject.SetActive(true); // �����̸� ��ư�� Ȱ��ȭ
        }
        else
        {
            button.gameObject.SetActive(false); // ������ �ƴϸ� ��ư�� ��Ȱ��ȭ
        }

        UpdateUpgradeUI();
    }

    // ��ž ���׷��̵� ����
    public void IncreaseStat(int turretIndex, string statType)
    {
        Turret turret = turrets[turretIndex];

        // �� ���׷��̵� ���� �� ���
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

    // ���� �ӵ� ���׷��̵� �Լ�
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
            UpdateUpgradeUI();  // ���׷��̵� UI ������Ʈ

            DisableUpgradeButtonWindow(turretIndex, "fireRate");
        }
    }

    // ������ ���׷��̵� �Լ�
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
            UpdateUpgradeUI();  // ���׷��̵� UI ������Ʈ

            DisableUpgradeButtonWindow(turretIndex, "damage");
        }
    }

    // ���� ���� ���׷��̵� �Լ�
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
            UpdateUpgradeUI();  // ���׷��̵� UI ������Ʈ

            DisableUpgradeButtonWindow(turretIndex, "explosionRadius");
        }
    }

    private void DisableUpgradeButtonWindow(int turretIndex, string upgradeType)
    {
        // ���׷��̵� Ÿ�Կ� ���� ������ ���׷��̵� ��ư â�� ���� ��ư�� ��Ȱ��ȭ
        if (turretIndex == 0) // ù ��° Ÿ��
        {
            if (upgradeType == "fireRate" && turretCurrentUpgrades[turretIndex] >= turretMaxUpgrades)
                upgradeWindowButtons[0].interactable = false;
            else if (upgradeType == "damage" && damageCurrentUpgrades[turretIndex] >= damageMaxUpgrades)
                upgradeWindowButtons[1].interactable = false;
            else if (upgradeType == "explosionRadius" && explosionRadiusCurrentUpgrades[turretIndex] >= explosionRadiusMaxUpgrades)
                upgradeWindowButtons[2].interactable = false;
        }
        else if (turretIndex == 1) // �� ��° Ÿ��
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
            // ���׷��̵� Ƚ�� �ؽ�Ʈ ������Ʈ
            if (fireRateUpgradeTexts[i] != null)
                fireRateUpgradeTexts[i].text = "" + turretCurrentUpgrades[i] + "/" + turretMaxUpgrades;

            if (damageUpgradeTexts[i] != null)
                damageUpgradeTexts[i].text = "" + damageCurrentUpgrades[i] + "/" + damageMaxUpgrades;

            if (explosionRadiusUpgradeTexts[i] != null)
                explosionRadiusUpgradeTexts[i].text = "" + explosionRadiusCurrentUpgrades[i] + "/" + explosionRadiusMaxUpgrades;

            // ��ȭ ��� �ؽ�Ʈ ������Ʈ
            if (fireRateCostTexts[i] != null)
                fireRateCostTexts[i].text = "��ȭ ���: " + turretUpgradeCost;
            if (damageCostTexts[i] != null)
                damageCostTexts[i].text = "��ȭ ���: " + damageUpgradeCost;
            if (explosionRadiusCostTexts[i] != null)
                explosionRadiusCostTexts[i].text = "��ȭ ���: " + explosionRadiusUpgradeCost;
        }
    }

    // RPC�� ��� Ŭ���̾�Ʈ�� ���׷��̵� ���� ����ȭ
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

        UpdateUpgradeUI();  // UI ����ȭ
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

        UpdateUpgradeUI();  // UI ����ȭ
    }
}