using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

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

    void Start()
    {
         // ����� ���������ε�
        userId = PlayerPrefs.GetString("Player", $"USER_{Random.Range(1, 21):00}");
        userIF.text = userId;

        // ���� ������ �г��ӵ��
        PhotonNetwork.NickName = userId;
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
}