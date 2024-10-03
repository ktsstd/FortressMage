using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartSettingUi : PhotonManager
{
    public Transform[] spawnPoint;
    public GameObject[] wizards;

    public TextMeshProUGUI[] loading;
    public TextMeshProUGUI mageSelect;

    private string chooseMage = "";

    public void Update()
    {
        if (isjoin)
        {
            loading[0].text = "로딩 중"; // 로딩 중 상태 표시
        }
    }

    public void ChooseFire()
    {
        chooseMage = "FireWizard"; // 선택한 마법사 설정
        mageSelect.text = "파이어 위자드 선택됨"; // 선택된 마법사 텍스트 표시
    }

    public void CloseUiandPlay()
    {
        if (isjoin && chooseMage != "")
        {
            int idx = Random.Range(0, spawnPoint.Length);

            // 선택한 마법사를 랜덤한 스폰 지점에 인스턴스화
            PhotonNetwork.Instantiate(chooseMage, spawnPoint[idx].position, spawnPoint[idx].rotation, 0);
            gameObject.SetActive(false); // UI 비활성화
        }
        else
        {
            loading[1].text = "선택한 마법사가 없습니다"; // 마법사 선택 오류 메시지
        }
    }
}