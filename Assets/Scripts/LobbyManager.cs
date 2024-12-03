using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks

{
    public TMP_Text roomNameText;
    public TMP_Text[] playerTexts;
    public Button exitBtn;
    public TMP_Text messageText;

    private List<Photon.Realtime.Player> playerList = new List<Photon.Realtime.Player>();  // 방에 있는 플레이어 리스트

    public Image[] targetImage;  // 여러 이미지 컴포넌트를 배열로 관리

    public Sprite[] newSprites;  // 교체할 이미지들

    private bool isConfirmed = false; 

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


         if (messageText != null)
        {
            string msg = $"\n<color=#00ff00>{newPlayer.NickName}</color> 님이 방에 들어왔습니다!";
            messageText.text += msg; // 메시지를 계속 추가
        }
    }

    // 방에서 플레이어가 나가면 텍스트를 초기화할 수도 있습니다.
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        playerList.Remove(otherPlayer);
        UpdatePlayerListUI();


        if (messageText != null)
        {
            string msg = $"\n<color=#ff0000>{otherPlayer.NickName}</color> 님이 방을 나갔습니다.";
            messageText.text += msg; // 메시지를 계속 추가
        }
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

    public void OnImageSwitchButtonClick(int imageIndex)
    {
        if (isConfirmed) return; // 확정되었으면 이미지 변경을 할 수 없도록

        // 로컬 플레이어의 번호에 따라 업데이트
        int targetPlayer = PhotonNetwork.LocalPlayer.ActorNumber;
        UpdateImageForPlayer(imageIndex, targetPlayer);
    }

    private void UpdateImageForPlayer(int buttonIndex, int playerNumber)
    {
        if (newSprites == null || newSprites.Length < 2 || playerNumber < 1 || playerNumber > 4) return;

        int startIndex = buttonIndex * 2;

        // 1번 타겟 이미지는 모든 플레이어에게 동일
        if (startIndex < newSprites.Length && targetImage[0] != null)
            targetImage[0].sprite = newSprites[startIndex];

        // 플레이어에 따라 타겟 이미지 변경 (playerNumber 1, 2, 3, 4에 대해 각각 2번, 3번, 4번, 5번 타겟 변경)
        int targetIndex = playerNumber;  // 1번 -> 1, 2번 -> 2, 3번 -> 3, 4번 -> 4

        if (targetIndex < targetImage.Length && startIndex + 1 < newSprites.Length && targetImage[targetIndex] != null)
            targetImage[targetIndex].sprite = newSprites[startIndex + 1];
    }

    public void OnConfirmButtonClick()
    {
        isConfirmed = true; // 확정 버튼이 눌리면 캐릭터 변경을 못하게 설정
    }
}