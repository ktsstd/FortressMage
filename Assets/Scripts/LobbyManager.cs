using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks

{
    public TMP_Text roomNameText;
    public TMP_Text[] playerTexts;
    public Button exitBtn;

    private List<Photon.Realtime.Player> playerList = new List<Photon.Realtime.Player>();  // 방에 있는 플레이어 리스트


    void Awake()
    {
        exitBtn.onClick.AddListener(() => OnExitClick());
    }

    private void OnExitClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    // 포톤 룸에서 퇴장했을 때 호출되는 콜백함수
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("New Scene"); // 퇴장 후 씬 전환
    }

    void Start()
    {
        // 방 이름을 텍스트에 표시
        if (roomNameText != null)
        {
            roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        }

        // 플레이어 목록에 현재 방의 모든 플레이어를 추가
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            playerList.Add(player);
        }

        // UI 업데이트
        UpdatePlayerListUI();
    }

    // 방에 새로 플레이어가 입장하면 이 메서드가 호출됩니다.
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        playerList.Add(newPlayer);
        UpdatePlayerListUI();
    }

    // 방에서 플레이어가 나가면 텍스트를 초기화할 수도 있습니다.
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        playerList.Remove(otherPlayer);
        UpdatePlayerListUI();
    }

    // 플레이어 리스트를 기반으로 UI를 업데이트하는 함수
    void UpdatePlayerListUI()
    {
        // 플레이어 텍스트 배열을 모두 초기화 (빈 텍스트로 설정)
        for (int i = 0; i < playerTexts.Length; i++)
        {
            playerTexts[i].text = "";
        }

        // 방에 있는 모든 플레이어의 닉네임을 텍스트로 추가
        for (int i = 0; i < playerList.Count && i < playerTexts.Length; i++)
        {
            playerTexts[i].text = playerList[i].NickName;
        }
    }

    // 방장이 나갔을 때 호출되는 콜백 함수
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        UpdatePlayerListUI(); // 새로운 방장 정보 갱신
    }
}