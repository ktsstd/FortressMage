using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements.Experimental;

public class PhotonManager : MonoBehaviourPunCallbacks //PUN�� �پ��� �ݹ� �Լ��� �������̵��ؼ� �ۼ�
{
    //���� ����
    private readonly string version = "1.0";
    //���� �г���
    private string userId = "Player";

    public bool isjoin = false;
    void Awake()
    {
        //������ Ŭ���̾�Ʈ(��������������)�Ǿ��ڵ�����ȭ�ɼ�
        PhotonNetwork.AutomaticallySyncScene = true;
        //���� ���� ����(���Ϲ�������������������)
        PhotonNetwork.GameVersion = version;
        //���� �����Ǵг��Ӽ���
        PhotonNetwork.NickName = userId;
        //���� �������ǵ��������ʴ�����Ƚ��(�⺻30ȸ)
        Debug.Log("PhotonNetwork.SendRate : " + PhotonNetwork.SendRate);
        //���� ���� ����
        Debug.Log("1) ���� ���� ����");
        PhotonNetwork.ConnectUsingSettings();
    }
    //���� ������������ȣ��Ǵ��ݹ��Լ�
    public override void OnConnectedToMaster()
    {
        Debug.Log("2) ���� ���� ���� �� ȣ��Ǵ� �ݹ� �Լ�");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); //�ڵ� ������ �ƴϹǷ� false
        Debug.Log("3) �κ� ���� ���� OnJoinedLobby ȣ��");
        PhotonNetwork.JoinLobby(); //�κ� ���� ���� OnJoinedLobby ȣ��
    }
    //�κ� ������ȣ��Ǵ��ݹ��Լ�
    public override void OnJoinedLobby()
    {
        Debug.Log("4) �κ� ����");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        Debug.Log("5) �������� ������ �뿡 ����");
        PhotonNetwork.JoinRandomRoom();
    }
    //������ �� �����̽����������ȣ��Ǵ��ݹ��Լ�
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("�뿡 ���� ����");
        Debug.Log($"JoinRandom Failed = {returnCode}:{message}");
        //���� �Ӽ�����
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 4;
        ro.IsOpen = true;
        ro.IsVisible = true;
        //�뿡 �����Ҽ��ִ��ִ������ڼ�
        //���� ���¿���
        //�κ񿡼� ���Ͽ������ų������
        //�� ����
        Debug.Log("6) �� ����");
        PhotonNetwork.CreateRoom("YS Room", ro);
    }
    //�� ���� �Ϸ���ȣ��Ǵ��ݹ��Լ�
    public override void OnCreatedRoom()
    {
        Debug.Log("�� ���� �Ϸ� �� ȣ��Ǵ� �ݹ� �Լ�");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }
    //�뿡 ��������ȣ��Ǵ��ݹ��Լ�
    public override void OnJoinedRoom()
    {
        Debug.Log("�뿡 ������ �� ȣ��Ǵ� �ݹ� �Լ�");
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName} , {player.Value.ActorNumber}");
        }

        isjoin = true;
    }
}
