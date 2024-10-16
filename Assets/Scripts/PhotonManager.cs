using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    // ���� ��ȣ
    private readonly string version = "1.0";
    // ����� ID
    private string userId = "Player";

    public bool isjoin = false;

    void Awake()
    {
        // Photon Network�� �ڵ� ����ȭ ����
        PhotonNetwork.AutomaticallySyncScene = true;
        // ���� ���� ����
        PhotonNetwork.GameVersion = version;
        // ����� �̸� ����
        PhotonNetwork.NickName = userId;
        // ��Ʈ��ũ ���� �ӵ� Ȯ��
        Debug.Log("PhotonNetwork.SendRate : " + PhotonNetwork.SendRate);
        // ��Ʈ��ũ ���� ����
        Debug.Log("1) ��Ʈ��ũ ���� ����");
        PhotonNetwork.ConnectUsingSettings();
    }

    // ������ ������ ���� ����
    public override void OnConnectedToMaster()
    {
        Debug.Log("2) ������ ������ ���� ����");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); // �κ� �ִ��� Ȯ�� (false)
        Debug.Log("3) �κ� ���� �õ�");
        PhotonNetwork.JoinLobby(); // �κ� ����
    }

    // �κ� ���� ����
    public override void OnJoinedLobby()
    {
        Debug.Log("4) �κ� ���� ����");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        Debug.Log("5) ���� �濡 ���� �õ�");
        PhotonNetwork.JoinRandomRoom();
    }

    // ���� �� ���� ����
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("���� �� ���� ����");
        Debug.Log($"JoinRandom Failed = {returnCode}:{message}");

        // ���ο� �� ����
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 4;
        ro.IsOpen = true;
        ro.IsVisible = true;

        Debug.Log("6) ���ο� �� ���� �õ�");
        PhotonNetwork.CreateRoom("YS Room", ro);
    }

    // �� ���� ����
    public override void OnCreatedRoom()
    {
        Debug.Log("�� ���� ����");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    // �濡 ���� ����
    public override void OnJoinedRoom()
    {
        Debug.Log("�濡 ���� ����");
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName} , {player.Value.ActorNumber}");
        }

        isjoin = true;
    }
}