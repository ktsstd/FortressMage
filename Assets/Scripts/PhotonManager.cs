using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements.Experimental;

public class PhotonManager : MonoBehaviourPunCallbacks //PUN의 다양한 콜백 함수를 오버라이드해서 작성
{
    //게임 버전
    private readonly string version = "1.0";
    //유저 닉네임
    private string userId = "Player";

    public bool isjoin = false;
    void Awake()
    {
        //마스터 클라이언트(룸을생성한유저)의씬자동동기화옵션
        PhotonNetwork.AutomaticallySyncScene = true;
        //게임 버전 설정(동일버전의유저끼리컨넥팅)
        PhotonNetwork.GameVersion = version;
        //게임 유저의닉네임설정
        PhotonNetwork.NickName = userId;
        //포톤 서버와의데이터의초당전송횟수(기본30회)
        Debug.Log("PhotonNetwork.SendRate : " + PhotonNetwork.SendRate);
        //포톤 서버 접속
        Debug.Log("1) 포톤 서버 접속");
        PhotonNetwork.ConnectUsingSettings();
    }
    //포턴 서버에접속후호출되는콜백함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("2) 포턴 서버 접속 후 호출되는 콜백 함수");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); //자동 입장이 아니므로 false
        Debug.Log("3) 로비 입장 명령 OnJoinedLobby 호출");
        PhotonNetwork.JoinLobby(); //로비 입장 명령 OnJoinedLobby 호출
    }
    //로비에 접속후호출되는콜백함수
    public override void OnJoinedLobby()
    {
        Debug.Log("4) 로비 입장");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        Debug.Log("5) 무작위로 선택한 룸에 입장");
        PhotonNetwork.JoinRandomRoom();
    }
    //램덤한 룸 입장이실패했을경우호출되는콜백함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("룸에 입장 실패");
        Debug.Log($"JoinRandom Failed = {returnCode}:{message}");
        //룸의 속성정의
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 4;
        ro.IsOpen = true;
        ro.IsVisible = true;
        //룸에 입장할수있는최대접속자수
        //룸의 오픈여부
        //로비에서 룸목록에노출시킬지여부
        //룸 생성
        Debug.Log("6) 룸 생성");
        PhotonNetwork.CreateRoom("YS Room", ro);
    }
    //룸 생성 완료후호출되는콜백함수
    public override void OnCreatedRoom()
    {
        Debug.Log("룸 생성 완료 후 호출되는 콜백 함수");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }
    //룸에 입장한후호출되는콜백함수
    public override void OnJoinedRoom()
    {
        Debug.Log("룸에 입장한 후 호출되는 콜백 함수");
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName} , {player.Value.ActorNumber}");
        }

        isjoin = true;
    }
}

