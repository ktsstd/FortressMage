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
    public TMP_Text ReadyText;
    public Button readyBtn;
    public Button gameStartBtn;
    public TMP_Text[] readylistText;
    [SerializeField] private TMP_InputField nicknameText;


    private List<Player> playerList = new List<Player>();  // 방에 있는 플레이어 리스트
    private Dictionary<int, bool> playerReadyState = new Dictionary<int, bool>();
    private Dictionary<int, int> playerSelectedButtonIndex = new Dictionary<int, int>();

    public Button[] imageButtons;
    public Image targetImage;  // 여러 이미지 컴포넌트를 배열로 관리
    public Sprite[] newSprites;  // 교체할 이미지들

    private bool isConfirmed = false;
    private bool isReady = false;

    void Awake()
    {
        exitBtn.onClick.AddListener(() => OnExitClick());
        readyBtn.onClick.AddListener(OnReadyButtonClick);
        readyBtn.interactable = false;
        gameStartBtn.gameObject.SetActive(false);
        gameStartBtn.onClick.AddListener(OnGameStartButtonClick);
    }

    private void OnExitClick()
    {
        PlayerPrefs.SetString("PlayerNickname", PhotonNetwork.NickName);
        PhotonNetwork.LeaveRoom();
    }

    // 포톤 룸에서 퇴장했을 때 호출되는 콜백함수
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("New Scene"); // 퇴장 후 씬 전환
    }

    void Start()
    { 
        nicknameText.text = PhotonNetwork.NickName;
        // 방 이름을 텍스트에 표시
        if (roomNameText != null)
        {
            roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        }

        // 플레이어 목록에 현재 방의 모든 플레이어를 추가
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerList.Add(player);
            playerReadyState[player.ActorNumber] = false;
        }

        for (int i = 0; i < imageButtons.Length; i++)
        {
            int buttonIndex = i;  // 버튼 인덱스를 저장
            imageButtons[i].onClick.AddListener(() => OnImageSwitchButtonClick(buttonIndex));
        }
        // UI 업데이트
        UpdatePlayerListUI();
    }

    // 방에 새로 플레이어가 입장하면 이 메서드가 호출됩니다.
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        playerList.Add(newPlayer);
        playerReadyState[newPlayer.ActorNumber] = false;
        UpdatePlayerListUI();


         if (messageText != null)
        {
            string msg = $"\n<color=#00ff00>{newPlayer.NickName}</color> 님이 방에 들어왔습니다!";
            messageText.text += msg; // 메시지를 계속 추가
        }

        SyncButtonStatesWithNewPlayer(newPlayer);
    }

    // 방에서 플레이어가 나가면 텍스트를 초기화할 수도 있습니다.
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        playerList.Remove(otherPlayer);
        playerReadyState.Remove(otherPlayer.ActorNumber);
        playerSelectedButtonIndex.Remove(otherPlayer.ActorNumber);
        photonView.RPC("SyncButtonDeactivation", RpcTarget.All, -1);
        UpdatePlayerListUI();


        if (messageText != null)
        {
            string msg = $"\n<color=#ff0000>{otherPlayer.NickName}</color> 님이 방을 나갔습니다.";
            messageText.text += msg; // 메시지를 계속 추가
        }

        if (playerSelectedButtonIndex.ContainsKey(otherPlayer.ActorNumber))
        {
            playerSelectedButtonIndex.Remove(otherPlayer.ActorNumber);  // 나간 사람의 선택 삭제
        }
        UpdateButtonsForRemainingPlayers();
    }

    // 플레이어 리스트를 기반으로 UI를 업데이트하는 함수
    void UpdatePlayerListUI()
    {
        for (int i = 0; i < playerTexts.Length; i++)
        {
            playerTexts[i].text = "";
            readylistText[i].text = "";
        }

        // 방에 있는 모든 플레이어의 닉네임을 텍스트로 추가
        for (int i = 0; i < playerList.Count && i < playerTexts.Length; i++)
        {
            playerTexts[i].text = playerList[i].NickName;

            // 준비 상태를 해당 플레이어 순서에 맞게 업데이트
            int playerIndex = i;
            if (playerReadyState.ContainsKey(playerList[i].ActorNumber))
            {
                bool isPlayerReady = playerReadyState[playerList[i].ActorNumber];
                readylistText[playerIndex].text = isPlayerReady ? "준비 완료" : "";
            }

            // 선택된 버튼이 있으면 그 버튼 비활성화
            if (playerSelectedButtonIndex.ContainsKey(playerList[i].ActorNumber))
            {
                int selectedButtonIndex = playerSelectedButtonIndex[playerList[i].ActorNumber];
                if (isConfirmed && selectedButtonIndex >= 0 && selectedButtonIndex < imageButtons.Length)
                {
                    // 확정 버튼을 눌렀으면 그 버튼을 비활성화 상태로 표시
                    imageButtons[selectedButtonIndex].interactable = false;  // 해당 버튼 비활성화
                }
            }
        }
        CheckAllPlayersReady();
    }

    // 방장이 나갔을 때 호출되는 콜백 함수
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        UpdatePlayerListUI(); // 새로운 방장 정보 갱신
    }

    private void CheckAllPlayersReady()
    {
        bool allPlayersReady = true;

        // 모든 플레이어가 준비 완료 상태인지 확인
        foreach (var playerState in playerReadyState)
        {
            if (!playerState.Value)
            {
                allPlayersReady = false;
                break;
            }
        }

        // 방장이 준비 완료 버튼을 게임 시작 버튼으로 변경
        if (allPlayersReady && PhotonNetwork.IsMasterClient)
        {
            gameStartBtn.gameObject.SetActive(true);  // 게임 시작 버튼 활성화
            readyBtn.gameObject.SetActive(false);  // 준비 버튼 비활성화
        }
        else
        {
            gameStartBtn.gameObject.SetActive(false);  // 게임 시작 버튼 비활성화
            readyBtn.gameObject.SetActive(true);  // 준비 버튼 활성화
        }
    }

    public void OnImageSwitchButtonClick(int imageIndex)
    {
        if (isConfirmed) return; // 확정되었으면 이미지 변경을 못하게 설정

        UpdateImage(imageIndex);
    }

    private void UpdateImage(int buttonIndex)
    {
        if (newSprites == null || newSprites.Length < 4) return;

        // 버튼 클릭 시 이미지를 업데이트합니다.
        if (targetImage != null && buttonIndex < newSprites.Length)
        {
            targetImage.sprite = newSprites[buttonIndex];  // 해당 인덱스의 이미지를 타겟 이미지에 변경
            playerSelectedButtonIndex[PhotonNetwork.LocalPlayer.ActorNumber] = buttonIndex;  // 선택한 버튼 인덱스를 저장
        }
    }
    [PunRPC]
    public void UpdateButtonStateForAllClients(int buttonIndex)
    {
        if (buttonIndex >= 0 && buttonIndex < imageButtons.Length)
        {
            imageButtons[buttonIndex].interactable = false;
        }
    }

    public void OnConfirmButtonClick()
    {
        isConfirmed = true;  // 확정 버튼을 누르면 이제 비활성화

        int selectedButtonIndex = playerSelectedButtonIndex[PhotonNetwork.LocalPlayer.ActorNumber];

        // 확정 버튼 클릭 후 선택된 버튼만 비활성화
        if (selectedButtonIndex >= 0 && selectedButtonIndex < imageButtons.Length)
        {
            imageButtons[selectedButtonIndex].interactable = false;  // 해당 버튼을 비활성화합니다.

            // 이 상태를 다른 플레이어들에게 동기화합니다.
            photonView.RPC("SyncButtonDeactivation", RpcTarget.All, selectedButtonIndex);
        }

        readyBtn.interactable = true;
    }

    private void SyncButtonStatesWithNewPlayer(Photon.Realtime.Player newPlayer)
    {
        // 각 플레이어가 선택한 버튼을 새로 들어온 플레이어에게 전송
        foreach (var playerState in playerSelectedButtonIndex)
        {
            int playerActorNumber = playerState.Key;
            int selectedButtonIndex = playerState.Value;

            // 선택된 버튼을 비활성화
            photonView.RPC("SyncButtonDeactivation", newPlayer, selectedButtonIndex);
        }
    }

    [PunRPC]
    public void SyncButtonDeactivation(int buttonIndex)
    {
        // 선택된 버튼을 비활성화 상태로 만듦
        if (buttonIndex >= 0 && buttonIndex < imageButtons.Length)
        {
            imageButtons[buttonIndex].interactable = false;
        }
    }

    public void OnReadyButtonClick()
    {
        // 준비 상태 토글
        isReady = !isReady;

        // 로컬 플레이어의 준비 상태를 업데이트
        playerReadyState[PhotonNetwork.LocalPlayer.ActorNumber] = isReady;

        // 준비 상태에 따라 텍스트를 갱신
        if (ReadyText != null)
        {
            ReadyText.text = isReady ? "준비 완료" : "준비";
        }

        // UI 업데이트
        UpdatePlayerListUI();

        photonView.RPC("UpdateReadyState", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, isReady);
    }

    [PunRPC]
    public void UpdateReadyState(int playerActorNumber, bool readyStatus)
    {
        if (playerReadyState.ContainsKey(playerActorNumber))
        {
            playerReadyState[playerActorNumber] = readyStatus;
        }

        UpdatePlayerListUI();
    }

    [PunRPC]
    public void EnableButtonForAllClients(int buttonIndex)
    {
        if (buttonIndex >= 0 && buttonIndex < imageButtons.Length)
        {
            // 나간 사람이 선택한 버튼을 활성화 상태로 만들기
            imageButtons[buttonIndex].interactable = true;
        }
    }

    // 게임 시작 버튼 클릭 시 호출되는 함수
    private void OnGameStartButtonClick()
    {
        // 게임 시작 로직을 여기에 추가
        PhotonNetwork.LoadLevel("MultiplayScene"); // 예시: "GameScene"로 로드
    }

    public void OnChangeNicknameButtonClick()
    {
        string newNickname = nicknameText.text;

        // 닉네임이 변경되면 포톤 네트워크에 반영
        if (!string.IsNullOrEmpty(newNickname) && newNickname != PhotonNetwork.NickName)
        {
            PhotonNetwork.NickName = newNickname;  // 새로운 닉네임을 포톤 네트워크에 설정
            UpdatePlayerListUI();  // 플레이어 목록 UI 업데이트
            PlayerPrefs.SetString("Player", newNickname);

            photonView.RPC("UpdateNickname", RpcTarget.All, newNickname);
        }
    }

    [PunRPC]
    public void UpdateNickname(string newNickname)
    {
        // 닉네임 변경이 포톤 네트워크에 반영되었으므로, UI를 갱신
        nicknameText.text = newNickname;  // UI에서 닉네임 변경
        UpdatePlayerListUI();  // 플레이어 목록 UI 업데이트
    }

    private void UpdateButtonsForRemainingPlayers()
    {
        // 모든 버튼을 활성화 상태로 초기화
        foreach (var button in imageButtons)
        {
            button.interactable = true;
        }

        // 남아있는 플레이어들의 선택 상태를 반영하여 버튼 비활성화
        foreach (var playerState in playerSelectedButtonIndex)
        {
            int selectedButtonIndex = playerState.Value;

            // 이미 다른 플레이어가 선택한 버튼을 비활성화
            imageButtons[selectedButtonIndex].interactable = false;
        }
    }
}