using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    // 버전 번호
    private readonly string version = "1.0";
    // 사용자 ID
    private string userId = "Player";
    // 유저명을 입력할 TextMeshPro Input Field
    public TMP_InputField userIF;

    public bool isjoin = false;

    //룸 목록에 대한 데이터를 저장하기 위한 딕셔너리 자료형
    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
    // 룸 목록을 표시할 프리팹
    private GameObject roomItemPrefab;
    // RoomItem 프리팹이 추가될 ScrollContent
    public Transform scrollContent;

    void Start()
    {
         // 저장된 유저명을로드
        userId = PlayerPrefs.GetString("Player", $"USER_{Random.Range(1, 21):00}");
        userIF.text = userId;

        // 접속 유저의 닉네임등록
        PhotonNetwork.NickName = userId;
    }

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

        // RoomItem 프리팹 로드
        roomItemPrefab = Resources.Load<GameObject>("RoomItem");
         // 포톤 서버 접속
        if (PhotonNetwork.IsConnected == false)
        {
        PhotonNetwork.ConnectUsingSettings();
        }
    }

    // 마스터 서버에 연결 성공
    public override void OnConnectedToMaster()
    {
        //Debug.Log("2) 마스터 서버에 연결 성공");
        //Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); // 로비에 있는지 확인 (false)
        //Debug.Log("3) 로비에 참가 시도");
        PhotonNetwork.JoinLobby(); // 로비에 참가
    }

    // 로비에 참가 성공
    public override void OnJoinedLobby()
    {
        //Debug.Log("4) 로비에 참가 성공");
        //Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        //Debug.Log("5) 랜덤 방에 참가 시도");
        //PhotonNetwork.JoinRandomRoom();
    }

    // 랜덤 방 참가 실패
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //Debug.Log("랜덤 방 참가 실패");
        Debug.Log($"JoinRandom Failed = {returnCode}:{message}");

        OnMakeRoomClick();

        // 새로운 방 생성
        //RoomOptions ro = new RoomOptions();
        //ro.MaxPlayers = 4;
        //ro.IsOpen = true;
        //ro.IsVisible = true;

        //Debug.Log("6) 새로운 방 생성 시도");
        //PhotonNetwork.CreateRoom("YS Room", ro);
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
        //Debug.Log("방에 참가 성공");
        //Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        //Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MultiplayScene");
        }
        
    }

    // 유저명을 설정하는로직
    public void SetUserId()
    {
        if (string.IsNullOrEmpty(userIF.text))
        {
            userId = $"USER_{Random.Range(1,21):00}";
        }
        else
        {
            userId = userIF.text;
        }

        // 유저명 저장
        PlayerPrefs.SetString("Player", userId);
        // 접속 유저의 닉네임등록
        PhotonNetwork.NickName = userId;
    }
    // 룸 명의 입력여부를확인하는로직
    string SetRoomName()
    {
        return $"ROOM_{Random.Range(1,101):000}";
    }

    //룸 목록을 수신하는 콜백함수
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
         // 삭제된 RoomItem 프리팹을 저장할 임시변수
         GameObject tempRoom = null; 

         foreach (var roomInfo in roomList)
        {
            // 룸이 삭제된 경우
            if (roomInfo.RemovedFromList == true)
            {
                // 딕셔너리에서 룸 이름으로 검색해 저장된 RoomItem 프리팹를 추출
                rooms.TryGetValue(roomInfo.Name, out tempRoom);

                // RoomItem 프리팹 삭제
                Destroy(tempRoom);

                // 딕셔너리에서 해당 룸 이름의 데이터를 삭제
                rooms.Remove(roomInfo.Name);
            }
            else // 룸 정보가 변경된 경우
            {
                // 룸 이름이 딕셔너리에 없는 경우 새로 추가
                if (rooms.ContainsKey(roomInfo.Name) == false)
                {
                    // RoomInfo 프리팹을 scrollContent 하위에 생성
                    GameObject roomPrefab = Instantiate(roomItemPrefab, scrollContent);
                    // 룸 정보를 표시하기 위해 RoomInfo 정보 전달
                    roomPrefab.GetComponent<RoomData>().RoomInfo = roomInfo;

                    // 딕셔너리 자료형에 데이터 추가
                    rooms.Add(roomInfo.Name, roomPrefab);
                }
                else // 룸 이름이 딕셔너리에 없는 경우에 룸 정보를 갱신
                {
                    rooms.TryGetValue(roomInfo.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = roomInfo;
                }
            }
        }
    }

    public void OnMakeRoomClick()
    {
        // 유저명 저장
        SetUserId();

        // 룸의 속성 정의
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 4;     // 룸에 입장할 수 있는 최대 접속자 수
        ro.IsOpen = true;       // 룸의 오픈 여부
        ro.IsVisible = true;    // 로비에서 룸 목록에 노출시킬 여부


        // 룸 생성
        PhotonNetwork.CreateRoom(SetRoomName(), ro);
    }
}