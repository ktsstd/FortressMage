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
            loading[0].text = "로딩완료";
        }
    }

    public void ChooseFire()
    {
        chooseMage = "FireWizard";
        mageSelect.text = "불마법사 선택됨";
    }

    public void CloseUiandPlay()
    {
        if (isjoin && chooseMage != "")
        {
            int idx = Random.Range(0, spawnPoint.Length);

            PhotonNetwork.Instantiate(chooseMage, spawnPoint[idx].position, spawnPoint[idx].rotation, 0);
            gameObject.SetActive(false);
        }
        else
        {
            loading[1].text = "캐릭터를 선택해주세요";
        }
    }
}
