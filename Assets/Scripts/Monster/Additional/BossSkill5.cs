using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BossSkill5 : MonoBehaviour
{
    private Vector3 defaultPos;
    private PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        defaultPos = transform.position;
        pv.RPC("MoveDefault", RpcTarget.All);
    }

    [PunRPC]
    public void MoveDefault()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject[] playertag = GameObject.FindGameObjectsWithTag("Player");
            foreach(GameObject playerObj in playertag)
            {
                playerObj.transform.position = defaultPos;
                PlayerController playerScript = playerObj.GetComponent<PlayerController>();
                playerScript.OnPlayerStun(2f);
                PhotonNetwork.Instantiate("Additional/Boss_Skill_5_1", defaultPos, Quaternion.identity); // Quaternion.Euler(-90, 0, 0)
            }
        }
    }
}
