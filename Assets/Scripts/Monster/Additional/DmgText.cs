using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class DmgText : MonoBehaviour
{
    Text damageText;
    void Start()
    {
        damageText = GetComponent<Text>();                                                                                                                                                                                                                                                                                                                                                                          
    }

    public void ShowDamageMessage(string monsterdmged)
    {
        // 메시지 설정
        damageText.text = monsterdmged;

        // 일정 시간 후 메시지 숨기기
        Invoke("HideDamageMessage", 10f);
    }
    void HideDamageMessage()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
