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

    private string chooseMage = "FireWizard";

    public void ChooseFire()
    {
        chooseMage = "FireWizard";
    }

    public void CloseUiandPlay()
    {
        if (isjoin == true)
        {
            int idx = Random.Range(0, spawnPoint.Length);

            PhotonNetwork.Instantiate(chooseMage, spawnPoint[idx].position, spawnPoint[idx].rotation, 0);
            gameObject.SetActive(false);
        }
    }
}
