using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretUpgradeManager : MonoBehaviour
{
    // 공통 변수
    public GoldManager goldManager;  // 골드 매니저 참조

    // 두 개의 Turret 관련 변수
    public Turret turret1;  // 첫 번째 포탑 참조
    public Turret turret2;  // 두 번째 포탑 참조
    public Button increaseFireRateButton1;  // 첫 번째 포탑의 공격 속도 업그레이드 버튼
    public Button increaseFireRateButton2;  // 두 번째 포탑의 공격 속도 업그레이드 버튼
    public Button increaseDamageButton1;    // 첫 번째 포탑의 데미지 업그레이드 버튼
    public Button increaseDamageButton2;    // 두 번째 포탑의 데미지 업그레이드 버튼
    public Button upgradeRadiusButton1;     // 첫 번째 포탑의 범위 업그레이드 버튼
    public Button upgradeRadiusButton2;     // 두 번째 포탑의 범위 업그레이드 버튼

    // 업그레이드 비용 및 제한 관련 변수
    public int turretUpgradeCost = 50;  // 포탑 업그레이드 비용
    public int turretMaxUpgrades = 5;   // 포탑 최대 업그레이드 횟수
    public float turretCostIncreaseRate = 1.5f;  // 포탑 업그레이드 비용 증가율
    private int[] turretCurrentUpgrades = { 0, 0 };  // 각 포탑의 업그레이드 횟수 (배열로 관리)

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
        // 첫 번째 포탑 버튼 설정
        if (increaseFireRateButton1 != null)
        {
            increaseFireRateButton1.onClick.AddListener(() => IncreaseFireRate(0, turret1));  // 첫 번째 포탑 업그레이드
        }

        if (increaseDamageButton1 != null)
        {
            increaseDamageButton1.onClick.AddListener(() => IncreaseDamage(0, turret1));  // 첫 번째 포탑 데미지 업그레이드
        }

        if (upgradeRadiusButton1 != null)
        {
            upgradeRadiusButton1.onClick.AddListener(() => UpgradeExplosionRadius(0, turret1));  // 첫 번째 포탑 범위 업그레이드
        }

        // 두 번째 포탑 버튼 설정
        if (increaseFireRateButton2 != null)
        {
            increaseFireRateButton2.onClick.AddListener(() => IncreaseFireRate(1, turret2));  // 두 번째 포탑 업그레이드
        }

        if (increaseDamageButton2 != null)
        {
            increaseDamageButton2.onClick.AddListener(() => IncreaseDamage(1, turret2));  // 두 번째 포탑 데미지 업그레이드
        }

        if (upgradeRadiusButton2 != null)
        {
            upgradeRadiusButton2.onClick.AddListener(() => UpgradeExplosionRadius(1, turret2));  // 두 번째 포탑 범위 업그레이드
        }
    }

    // 포탑의 공격 속도를 업그레이드하는 함수 (각 포탑에 대해 분리된 배열로 관리)
    public void IncreaseFireRate(int turretIndex, Turret turret)
    {
        // 포탑 업그레이드 제한 확인
        if (turretCurrentUpgrades[turretIndex] < turretMaxUpgrades)
        {
            if (goldManager.SpendGold(turretUpgradeCost))
            {
                turret.fireRate -= turret.fireRateIncreaseAmount;

                if (turret.fireRate < turret.maxFireRate)
                {
                    turret.fireRate = turret.maxFireRate;
                }

                turretCurrentUpgrades[turretIndex]++;  // 포탑 업그레이드 횟수 증가
                turretUpgradeCost = Mathf.RoundToInt(turretUpgradeCost * turretCostIncreaseRate);  // 업그레이드 비용 증가
            }
        }
    }

    // 포탑의 데미지를 업그레이드하는 함수
    public void IncreaseDamage(int turretIndex, Turret turret)
    {
        // 데미지 업그레이드 제한 확인
        if (damageCurrentUpgrades[turretIndex] < damageMaxUpgrades)
        {
            if (goldManager.SpendGold(damageUpgradeCost))
            {
                turret.damage += turret.increaseAmount;  // 데미지 증가
                damageCurrentUpgrades[turretIndex]++;  // 업그레이드 횟수 증가
                damageUpgradeCost = Mathf.RoundToInt(damageUpgradeCost * damageCostIncreaseRate);  // 업그레이드 비용 증가
            }
        }
    }

    // 포탑의 폭발 범위를 업그레이드하는 함수
    public void UpgradeExplosionRadius(int turretIndex, Turret turret)
    {
        // 범위 업그레이드 제한 확인
        if (explosionRadiusCurrentUpgrades[turretIndex] < explosionRadiusMaxUpgrades)
        {
            if (goldManager.SpendGold(explosionRadiusUpgradeCost))
            {
                turret.explosionRadius += explosionRadiusUpgradeAmount;  // 폭발 범위 증가
                explosionRadiusCurrentUpgrades[turretIndex]++;  // 업그레이드 횟수 증가
                explosionRadiusUpgradeCost = Mathf.RoundToInt(explosionRadiusUpgradeCost * 1.5f);  // 업그레이드 비용 증가
            }
        }
    }
}