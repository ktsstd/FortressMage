using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    // 버전 번호
    private readonly string version = "1.0";
    // 사용자 ID
    private string userId = "Player";

    public bool isjoin = false;

    void Awake()
    {
        // Photon Network의 자동 동기화 설정
        PhotonNetwork.AutomaticallySyncScene = true;
        // 게임 버전 설정
        PhotonNetwork.GameVersion = version;
        // 사용자 이름 설정
        PhotonNetwork.NickName = userId;
        // 네트워크 전송 속도 확인
        Debug.Log("PhotonNetwork.SendRate : " + PhotonNetwork.SendRate);
        // 네트워크 연결 시작
        Debug.Log("1) 네트워크 연결 시작");
        PhotonNetwork.ConnectUsingSettings();
    }

    // 마스터 서버에 연결 성공
    public override void OnConnectedToMaster()
    {
        Debug.Log("2) 마스터 서버에 연결 성공");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); // 로비에 있는지 확인 (false)
        Debug.Log("3) 로비에 참가 시도");
        PhotonNetwork.JoinLobby(); // 로비에 참가
    }

    // 로비에 참가 성공
    public override void OnJoinedLobby()
    {
        Debug.Log("4) 로비에 참가 성공");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        Debug.Log("5) 랜덤 방에 참가 시도");
        PhotonNetwork.JoinRandomRoom();
    }

    // 랜덤 방 참가 실패
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("랜덤 방 참가 실패");
        Debug.Log($"JoinRandom Failed = {returnCode}:{message}");

        // 새로운 방 생성
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 4;
        ro.IsOpen = true;
        ro.IsVisible = true;

        Debug.Log("6) 새로운 방 생성 시도");
        PhotonNetwork.CreateRoom("YS Room", ro);
    }

    // 방 생성 성공
    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 성공");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    // 방에 참가 성공
    public override void OnJoinedRoom()
    {
        Debug.Log("방에 참가 성공");
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName} , {player.Value.ActorNumber}");
        }

        isjoin = true;
    }
}