using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    // ���� ��ȣ
    private readonly string version = "1.0";
    // ����� ID
    private string userId = "Player";
    // �������� �Է��� TextMeshPro Input Field
    public TMP_InputField userIF;

    public bool isjoin = false;

    //�� ��Ͽ� ���� �����͸� �����ϱ� ���� ��ųʸ� �ڷ���
    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
    // �� ����� ǥ���� ������
    private GameObject roomItemPrefab;
    // RoomItem �������� �߰��� ScrollContent
    public Transform scrollContent;

    [SerializeField] private Button enterWaitingRoomButton;
    [SerializeField] private Button refreshButton;

    void Start()
    {
         // ����� ���������ε�
        userId = PlayerPrefs.GetString("Player", $"USER_{Random.Range(1, 21):00}");
        userIF.text = userId;

        // ���� ������ �г��ӵ��
        PhotonNetwork.NickName = userId;
        refreshButton.onClick.AddListener(ClearAndRefreshRoomList);
    }

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

        // RoomItem ������ �ε�
        roomItemPrefab = Resources.Load<GameObject>("RoomItem");
         // ���� ���� ����
        if (PhotonNetwork.IsConnected == false)
        {
        PhotonNetwork.ConnectUsingSettings();
        }
    }

    // ������ ������ ���� ����
    public override void OnConnectedToMaster()
    {
        //Debug.Log("2) ������ ������ ���� ����");
        //Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); // �κ� �ִ��� Ȯ�� (false)
        //Debug.Log("3) �κ� ���� �õ�");
        PhotonNetwork.JoinLobby(); // �κ� ����
    }

    // �κ� ���� ����
    public override void OnJoinedLobby()
    {
        //Debug.Log("4) �κ� ���� ����");
        //Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        //Debug.Log("5) ���� �濡 ���� �õ�");
        //PhotonNetwork.JoinRandomRoom();
        foreach (var room in rooms.Values)
        {
            Destroy(room);
        }
    }

    bool IsNicknameTaken(string nickname)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == nickname)
            {
                return true;
            }
        }
        return false;
    }

    // ���� �� ���� ����
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //Debug.Log("���� �� ���� ����");
        Debug.Log($"JoinRandom Failed = {returnCode}:{message}");

        OnMakeRoomClick();

        //���ο� �� ����
        //RoomOptions ro = new RoomOptions();
        //ro.MaxPlayers = 4;
        //ro.IsOpen = true;
        //ro.IsVisible = true;

        //Debug.Log("6) ���ο� �� ���� �õ�");
        //PhotonNetwork.CreateRoom("YS Room", ro);
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
        //Debug.Log("�濡 ���� ����");
        //Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        //Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MultiplayScene");
        }

    }

    // �������� �����ϴ·���
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

        // ������ ����
        PlayerPrefs.SetString("Player", userId);
        // ���� ������ �г��ӵ��
        PhotonNetwork.NickName = userId;
    }
    // �� ���� �Է¿��θ�Ȯ���ϴ·���
    string SetRoomName()
    {
        return $"ROOM_{Random.Range(1, 101):000}";
    }

    //�� ����� �����ϴ� �ݹ��Լ�
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 삭제된 방 아이템 초기화
    foreach (Transform child in scrollContent)
    {
        Destroy(child.gameObject); // 기존 방 아이템 삭제
    }

    // 방 목록 갱신
    foreach (var roomInfo in roomList)
    {
        if (!roomInfo.RemovedFromList)
        {
            // 새로운 방 아이템 생성
            GameObject roomPrefab = Instantiate(roomItemPrefab, scrollContent);
            RoomData roomData = roomPrefab.GetComponent<RoomData>();

            if (roomData != null)
            {
                roomData.RoomInfo = roomInfo;  // 방 정보 업데이트
            }

            rooms.Add(roomInfo.Name, roomPrefab);  // 방 아이템을 딕셔너리에 추가
        }
    }
    }

    public void ClearAndRefreshRoomList()
    {
        // 기존 방 아이템 삭제
        foreach (Transform child in scrollContent)
    {
        Destroy(child.gameObject); // 기존 방 아이템 삭제
    }

        rooms.Clear(); // rooms 딕셔너리 초기화

        // 방 목록을 다시 불러오고 갱신하도록 PhotonNetwork의 방 목록을 요청
        PhotonNetwork.JoinLobby();
    }


    public void ShowEnterRoomButton()
    {
        enterWaitingRoomButton.onClick.RemoveAllListeners();
        enterWaitingRoomButton.onClick.AddListener(() => OnEnterRoom());
    }

    public void OnEnterRoom()
    {
        string roomName = RoomData.selectedRoomName; // RoomData에서 방 이름을 받아옴

    // 실제로 해당 방에 입장
    PhotonNetwork.JoinRoom(roomName); // 해당 방에 입장
    }

    public void OnMakeRoomClick()
    {
        // ������ ����
        SetUserId();

        // ���� �Ӽ� ����
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 4;     // �뿡 ������ �� �ִ� �ִ� ������ ��
        ro.IsOpen = true;       // ���� ���� ����
        ro.IsVisible = true;    // �κ񿡼� �� ��Ͽ� �����ų ����


        // �� ����
        PhotonNetwork.CreateRoom(SetRoomName(), ro);
    }

    public override void OnLeftRoom()
{
    // 방을 떠날 때 로비로 돌아가서 방 목록을 갱신
    Debug.Log("Left the room");

    // 로비로 들어가서 방 목록 갱신
    PhotonNetwork.JoinLobby();

    // 기존 방 아이템 초기화
    foreach (var room in rooms.Values)
    {
        RoomData roomData = room.GetComponent<RoomData>();
        if (roomData != null)
        {
            roomData.ResetRoomData();  // 방 아이템의 데이터를 리셋
        }
    }
    
    rooms.Clear();  // 방 목록을 초기화
}
}