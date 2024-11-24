using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomData : MonoBehaviour
{
     private RoomInfo _roomInfo;
    // 하위에 있는 TMP_Text를 저장할 변수
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_Text roomPlayerCountText;

    // PhotonManager 접근 변수
    private PhotonManager photonManager;

    // 프로퍼티 정의
    public RoomInfo RoomInfo
    {
        get
        {
            return _roomInfo;
        }
        set
        {
            _roomInfo = value;
            // 룸 정보 표시
           roomNameText.text = _roomInfo.Name;
           roomPlayerCountText.text = $"({_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers})";

           // 버튼 클릭 이벤트에 함수 연결
           GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnEnterRoom(_roomInfo.Name));
        }
    }
    void Awake()
    {
        // 초기화 시 하위 컴포넌트 찾기
        if (roomNameText == null || roomPlayerCountText == null)
        {
            TMP_Text[] textComponents = GetComponentsInChildren<TMP_Text>();
            roomNameText = textComponents[0]; // 첫 번째 TMP_Text를 룸 이름으로 설정
            roomPlayerCountText = textComponents[1]; // 두 번째 TMP_Text를 인원수로 설정
        }

        photonManager = GameObject.Find("PhotonManager").GetComponent<PhotonManager>();
    }
    void OnEnterRoom(string roomName)
    {
        // 유저명 설정
        photonManager.SetUserId();
        // 룸 접속
        PhotonNetwork.JoinRoom(roomName);
    }
}