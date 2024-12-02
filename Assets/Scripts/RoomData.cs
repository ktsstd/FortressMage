using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomData : MonoBehaviour
{
     private RoomInfo _roomInfo;
    // ������ �ִ� TMP_Text�� ������ ����
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_Text roomPlayerCountText;
    [SerializeField] private TMP_Text playerNickText;

    // PhotonManager ���� ����
    private PhotonManager photonManager;

    // ������Ƽ ����
    public RoomInfo RoomInfo
    {
        get
        {
            return _roomInfo;
        }
        set
        {
            _roomInfo = value;
            // �� ���� ǥ��
           roomNameText.text = _roomInfo.Name;
           roomPlayerCountText.text = $"({_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers})";

            DisplayMasterClientName();

            // ��ư Ŭ�� �̺�Ʈ�� �Լ� ����
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnEnterRoom(_roomInfo.Name));
        }
    }
    void Awake()
    {
        // �ʱ�ȭ �� ���� ������Ʈ ã��
        if (roomNameText == null || roomPlayerCountText == null || playerNickText == null)
        {
            TMP_Text[] textComponents = GetComponentsInChildren<TMP_Text>();
            roomNameText = textComponents[0]; // 첫 번째 텍스트는 방 이름
            roomPlayerCountText = textComponents[1]; // 두 번째 텍스트는 플레이어 수
            playerNickText = textComponents[2]; // 세 번째 텍스트는 방을 만든 사람의 이름
        }

        photonManager = GameObject.Find("PhotonManager").GetComponent<PhotonManager>();
    }
    void OnEnterRoom(string roomName)
    {
        // ������ ����
        photonManager.SetUserId();
        // �� ����
        PhotonNetwork.JoinRoom(roomName);
    }
    void DisplayMasterClientName()
    {
        // 방에 있는 플레이어 리스트에서 MasterClient를 찾음
        if (PhotonNetwork.CurrentRoom != null)
        {
            // MasterClient의 ID를 통해 해당 플레이어의 닉네임을 가져옴
            int masterClientId = PhotonNetwork.CurrentRoom.MasterClientId;

            // 마스터 클라이언트가 방에 입장했을 때까지 기다림
            Player masterClient = PhotonNetwork.CurrentRoom.GetPlayer(masterClientId);

            // 방을 만든 사람의 이름을 표시
            if (masterClient != null)
            {
                playerNickText.text = masterClient.NickName;
            }
            else
            {
                playerNickText.text = "Unknown";
            }
        }
    }
}