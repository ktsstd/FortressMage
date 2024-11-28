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
        PhotonNetwork.Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // �� Ŭ���̾�Ʈ���� ���� ��ġ�� ȸ���� ����
            stream.SendNext(health);
        }
        else
        {
            // �ٸ� Ŭ���̾�Ʈ���� ���� ��ġ�� ȸ���� ����
            health = (float)stream.ReceiveNext();
        }
    }
}
