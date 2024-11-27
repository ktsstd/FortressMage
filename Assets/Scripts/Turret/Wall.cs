using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Wall : MonoBehaviourPunCallbacks, IPunObservable
{
    public float health = 100f;

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0f)
        {
            photonView.RPC("TowerTest", RpcTarget.All);
        }
    }

    [PunRPC]
    public void TowerTest()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 이 클라이언트에서 몬스터 위치와 회전을 전송
            stream.SendNext(health);
        }
        else
        {
            // 다른 클라이언트에서 몬스터 위치와 회전을 수신
            health = (float)stream.ReceiveNext();
        }
    }
}
