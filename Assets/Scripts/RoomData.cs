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

           // ��ư Ŭ�� �̺�Ʈ�� �Լ� ����
           GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnEnterRoom(_roomInfo.Name));
        }
    }
    void Awake()
    {
        // �ʱ�ȭ �� ���� ������Ʈ ã��
        if (roomNameText == null || roomPlayerCountText == null)
        {
            TMP_Text[] textComponents = GetComponentsInChildren<TMP_Text>();
            roomNameText = textComponents[0]; // ù ��° TMP_Text�� �� �̸����� ����
            roomPlayerCountText = textComponents[1]; // �� ��° TMP_Text�� �ο����� ����
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
}