using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartSettingUi : PhotonManager
{
    public Transform[] spawnPoint;

    public TextMeshProUGUI[] loading;
    public TextMeshProUGUI mageSelect;

    private string chooseMage = "";

    public void Update()
    {
        //if (isjoin)
            //loading[0].text = "로딩 완료";
        //else
            //loading[0].text = "로딩중";
    }

    public void ChooseFire()
    {
        chooseMage = "FireWizard"; // 선택한 마법사 설정
        mageSelect.text = "불마법사 선택됨"; // 선택된 마법사 텍스트 표시
    }
    public void ChooseIce()
    {
        chooseMage = "IceWizard";
        mageSelect.text = "얼음마법사 선택됨";
    }
    public void ChooseEarth()
    {
        chooseMage = "EarthWizard";
        mageSelect.text = "땅마법사 선택됨";
    }
    public void ChooseElectro()
    {
        chooseMage = "ElectroWizard";
        mageSelect.text = "번개마법사 선택됨";
    }

    public void CloseUiandPlay()
    {
        if (chooseMage != "")
        {
            int idx = Random.Range(0, spawnPoint.Length);

            // 선택한 마법사를 랜덤한 스폰 지점에 인스턴스화
            PhotonNetwork.Instantiate("Player/" + chooseMage, spawnPoint[idx].position, spawnPoint[idx].rotation, 0);
            gameObject.SetActive(false); // UI 비활성화
        }
        else
        {
            loading[1].text = "마법사를 선택하세요"; // 마법사 선택 오류 메시지
        }
    }
}