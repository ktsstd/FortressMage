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
    public Button confirmBtn;
    private bool isFirstImageUpdated = false;


    void Awake()
    {
        exitBtn.onClick.AddListener(() => OnExitClick());
        confirmBtn.onClick.AddListener(() => OnConfirmClick());  // 확정 버튼 클릭 시 호출될 함수 등록
    }
    private void OnConfirmClick()
{
    // 두 번째 이미지를 변경하는 함수 호출
    UpdateSecondImageLocally();
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
    // 버튼 클릭 시 호출되는 함수 (이미지를 변경하는 함수)
    public void OnImageSwitchButtonClick()
    {
        // 이미지를 변경하는 함수 호출
        UpdateImagesLocally();
    }

    private void UpdateImagesLocally()
{
    if (newSprites != null && newSprites.Length > 0)
    {
        if (targetImage.Length > 0 && targetImage[0] != null)
        {
            targetImage[0].sprite = newSprites[0];
            isFirstImageUpdated = true;  // 첫 번째 이미지가 변경되었음을 표시
        }
    }
}
private void UpdateSecondImageLocally()
{
    // 첫 번째 이미지가 변경된 후, 두 번째 이미지를 변경
    if (isFirstImageUpdated && newSprites != null && newSprites.Length > 1)
    {
        if (targetImage.Length > 1 && targetImage[1] != null)
        {
            targetImage[1].sprite = newSprites[1];
        }
    }
}
}