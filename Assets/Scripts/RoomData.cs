using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class RoomData : MonoBehaviour
{
     private RoomInfo _roomInfo;
    // ������ �ִ� TMP_Text�� ������ ����
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_Text roomPlayerCountText;

    // PhotonManager ���� ����
    private PhotonManager photonManager;
    public static string selectedRoomName;

    private static Button lastSelectedButton = null;

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
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnRoomSelected());
        }
    }
    void Awake()
    {
        // �ʱ�ȭ �� ���� ������Ʈ ã��
        if (roomNameText == null || roomPlayerCountText == null )
        {
            TMP_Text[] textComponents = GetComponentsInChildren<TMP_Text>();
            roomNameText = textComponents[0]; // 첫 번째 텍스트는 방 이름
            roomPlayerCountText = textComponents[1]; // 두 번째 텍스트는 플레이어 수=
        }

        photonManager = GameObject.Find("PhotonManager").GetComponent<PhotonManager>();
    }

    public void ResetRoomData()
{
    // RoomInfo 초기화
    _roomInfo = null;

    // UI 텍스트 초기화
    if (roomNameText != null) roomNameText.text = "";
    if (roomPlayerCountText != null) roomPlayerCountText.text = "(0/0)";
    
    // 버튼을 활성화 상태로 되돌리기 (방을 나갈 때)
    Button selectedButton = GetComponent<Button>();
    if (selectedButton != null)
    {
        selectedButton.interactable = true;  // 버튼을 다시 활성화 상태로 변경
    }
}

    void OnRoomSelected()
    {
        // 방 선택 시 해당 방 이름을 저장
        if (lastSelectedButton != null)
        {
            lastSelectedButton.interactable = true;  // 이전 버튼 활성화
        }

        // 현재 선택된 버튼을 비활성화
        Button selectedButton = GetComponent<Button>();
        if (selectedButton != null)
        {
            selectedButton.interactable = false;  // 현재 버튼 비활성화
        }

        // 선택된 버튼을 추적
        lastSelectedButton = selectedButton;

        // 방 이름 저장
        selectedRoomName = _roomInfo.Name;

        // 대기실 입장 버튼을 보여줌
        photonManager.ShowEnterRoomButton();
    }
}