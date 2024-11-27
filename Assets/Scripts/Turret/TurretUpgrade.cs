using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretUpgradeManager : MonoBehaviour
{
    // ���� ����
    public GoldManager goldManager;  // ��� �Ŵ��� ����

    // �� ���� Turret ���� ����
    public Turret turret1;  // ù ��° ��ž ����
    public Turret turret2;  // �� ��° ��ž ����
    public Button increaseFireRateButton1;  // ù ��° ��ž�� ���� �ӵ� ���׷��̵� ��ư
    public Button increaseFireRateButton2;  // �� ��° ��ž�� ���� �ӵ� ���׷��̵� ��ư
    public Button increaseDamageButton1;    // ù ��° ��ž�� ������ ���׷��̵� ��ư
    public Button increaseDamageButton2;    // �� ��° ��ž�� ������ ���׷��̵� ��ư
    public Button upgradeRadiusButton1;     // ù ��° ��ž�� ���� ���׷��̵� ��ư
    public Button upgradeRadiusButton2;     // �� ��° ��ž�� ���� ���׷��̵� ��ư

    // ���׷��̵� ��� �� ���� ���� ����
    public int turretUpgradeCost = 50;  // ��ž ���׷��̵� ���
    public int turretMaxUpgrades = 5;   // ��ž �ִ� ���׷��̵� Ƚ��
    public float turretCostIncreaseRate = 1.5f;  // ��ž ���׷��̵� ��� ������
    private int[] turretCurrentUpgrades = { 0, 0 };  // �� ��ž�� ���׷��̵� Ƚ�� (�迭�� ����)

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
        // ù ��° ��ž ��ư ����
        if (increaseFireRateButton1 != null)
        {
            increaseFireRateButton1.onClick.AddListener(() => IncreaseFireRate(0, turret1));  // ù ��° ��ž ���׷��̵�
        }

        if (increaseDamageButton1 != null)
        {
            increaseDamageButton1.onClick.AddListener(() => IncreaseDamage(0, turret1));  // ù ��° ��ž ������ ���׷��̵�
        }

        if (upgradeRadiusButton1 != null)
        {
            upgradeRadiusButton1.onClick.AddListener(() => UpgradeExplosionRadius(0, turret1));  // ù ��° ��ž ���� ���׷��̵�
        }

        // �� ��° ��ž ��ư ����
        if (increaseFireRateButton2 != null)
        {
            increaseFireRateButton2.onClick.AddListener(() => IncreaseFireRate(1, turret2));  // �� ��° ��ž ���׷��̵�
        }

        if (increaseDamageButton2 != null)
        {
            increaseDamageButton2.onClick.AddListener(() => IncreaseDamage(1, turret2));  // �� ��° ��ž ������ ���׷��̵�
        }

        if (upgradeRadiusButton2 != null)
        {
            upgradeRadiusButton2.onClick.AddListener(() => UpgradeExplosionRadius(1, turret2));  // �� ��° ��ž ���� ���׷��̵�
        }
    }

    // ��ž�� ���� �ӵ��� ���׷��̵��ϴ� �Լ� (�� ��ž�� ���� �и��� �迭�� ����)
    public void IncreaseFireRate(int turretIndex, Turret turret)
    {
        // ��ž ���׷��̵� ���� Ȯ��
        if (turretCurrentUpgrades[turretIndex] < turretMaxUpgrades)
        {
            if (goldManager.SpendGold(turretUpgradeCost))
            {
                turret.fireRate -= turret.fireRateIncreaseAmount;

                if (turret.fireRate < turret.maxFireRate)
                {
                    turret.fireRate = turret.maxFireRate;
                }

                turretCurrentUpgrades[turretIndex]++;  // ��ž ���׷��̵� Ƚ�� ����
                turretUpgradeCost = Mathf.RoundToInt(turretUpgradeCost * turretCostIncreaseRate);  // ���׷��̵� ��� ����
            }
        }
    }

    // ��ž�� �������� ���׷��̵��ϴ� �Լ�
    public void IncreaseDamage(int turretIndex, Turret turret)
    {
        // ������ ���׷��̵� ���� Ȯ��
        if (damageCurrentUpgrades[turretIndex] < damageMaxUpgrades)
        {
            if (goldManager.SpendGold(damageUpgradeCost))
            {
                turret.damage += turret.increaseAmount;  // ������ ����
                damageCurrentUpgrades[turretIndex]++;  // ���׷��̵� Ƚ�� ����
                damageUpgradeCost = Mathf.RoundToInt(damageUpgradeCost * damageCostIncreaseRate);  // ���׷��̵� ��� ����
            }
        }
    }

    // ��ž�� ���� ������ ���׷��̵��ϴ� �Լ�
    public void UpgradeExplosionRadius(int turretIndex, Turret turret)
    {
        // ���� ���׷��̵� ���� Ȯ��
        if (explosionRadiusCurrentUpgrades[turretIndex] < explosionRadiusMaxUpgrades)
        {
            if (goldManager.SpendGold(explosionRadiusUpgradeCost))
            {
                turret.explosionRadius += explosionRadiusUpgradeAmount;  // ���� ���� ����
                explosionRadiusCurrentUpgrades[turretIndex]++;  // ���׷��̵� Ƚ�� ����
                explosionRadiusUpgradeCost = Mathf.RoundToInt(explosionRadiusUpgradeCost * 1.5f);  // ���׷��̵� ��� ����
            }
        }
    }
}