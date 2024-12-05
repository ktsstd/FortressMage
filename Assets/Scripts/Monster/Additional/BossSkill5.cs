using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BossSkill5 : MonoBehaviour
{
    private Vector3 defaultPos;
    private Vector3 SpawnPos;
    private PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        defaultPos = transform.position;
        SpawnPos = new Vector3(defaultPos.x, defaultPos.y + 50, defaultPos.z);
        // pv.RPC("MoveDefault", RpcTarget.All);
        MoveDefault();
    }

    // [PunRPC]
    public void MoveDefault()
    {
        GameObject[] playertag = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject playerObj in playertag)
        {
            playerObj.transform.position = defaultPos; 
            PlayerController playerScript = playerObj.GetComponent<PlayerController>();
            playerScript.OnPlayerStun(2.1f);
        }
        StartCoroutine(StartBoss5D());
        // if (PhotonNetwork.IsMasterClient)
        // {
        //     GameObject[] playertag = GameObject.FindGameObjectsWithTag("Player");
        //     foreach(GameObject playerObj in playertag)
        //     {
        //         playerObj.transform.position = defaultPos; 
        //     }
        // }
        // PlayerController playerScript = playerObj.GetComponent<PlayerController>();
        // playerScript.OnPlayerStun(2f);
        // PhotonNetwork.Instantiate("Additional/Boss_Skill_5-1", defaultPos, Quaternion.identity); // Quaternion.Euler(-90, 0, 0)
        // PhotonNetwork.Destroy(gameObject);
    }

    private IEnumerator StartBoss5D()
    {
        PhotonNetwork.Instantiate("Additional/Boss_Skill_5-1", SpawnPos, Quaternion.Euler(90, 0, 0)); // Quaternion.identity
        yield return new WaitForSeconds(2.8f);
        PhotonNetwork.Destroy(gameObject);
    }
}
