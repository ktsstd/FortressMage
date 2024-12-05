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


    private List<Player> playerList = new List<Player>();  // �濡 �ִ� �÷��̾� ����Ʈ
    private Dictionary<int, bool> playerReadyState = new Dictionary<int, bool>();
    private Dictionary<int, int> playerSelectedButtonIndex = new Dictionary<int, int>();

    public Button[] imageButtons;
    public Image targetImage;  // ���� �̹��� ������Ʈ�� �迭�� ����
    public Sprite[] newSprites;  // ��ü�� �̹�����

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

    // ���� �뿡�� �������� �� ȣ��Ǵ� �ݹ��Լ�
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("New Scene"); // ���� �� �� ��ȯ
    }

    void Start()
    { 
        nicknameText.text = PhotonNetwork.NickName;
        // �� �̸��� �ؽ�Ʈ�� ǥ��
        if (roomNameText != null)
        {
            roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        }

        // �÷��̾� ��Ͽ� ���� ���� ��� �÷��̾ �߰�
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerList.Add(player);
            playerReadyState[player.ActorNumber] = false;
        }

        for (int i = 0; i < imageButtons.Length; i++)
        {
            int buttonIndex = i;  // ��ư �ε����� ����
            imageButtons[i].onClick.AddListener(() => OnImageSwitchButtonClick(buttonIndex));
        }
        // UI ������Ʈ
        UpdatePlayerListUI();
    }

    // �濡 ���� �÷��̾ �����ϸ� �� �޼��尡 ȣ��˴ϴ�.
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        playerList.Add(newPlayer);
        playerReadyState[newPlayer.ActorNumber] = false;
        UpdatePlayerListUI();


         if (messageText != null)
        {
            string msg = $"\n<color=#00ff00>{newPlayer.NickName}</color> ���� �濡 ���Խ��ϴ�!";
            messageText.text += msg; // �޽����� ��� �߰�
        }

        SyncButtonStatesWithNewPlayer(newPlayer);
    }

    // �濡�� �÷��̾ ������ �ؽ�Ʈ�� �ʱ�ȭ�� ���� �ֽ��ϴ�.
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        playerList.Remove(otherPlayer);
        playerReadyState.Remove(otherPlayer.ActorNumber);
        playerSelectedButtonIndex.Remove(otherPlayer.ActorNumber);
        photonView.RPC("SyncButtonDeactivation", RpcTarget.All, -1);
        UpdatePlayerListUI();


        if (messageText != null)
        {
            string msg = $"\n<color=#ff0000>{otherPlayer.NickName}</color> ���� ���� �������ϴ�.";
            messageText.text += msg; // �޽����� ��� �߰�
        }

        if (playerSelectedButtonIndex.ContainsKey(otherPlayer.ActorNumber))
        {
            playerSelectedButtonIndex.Remove(otherPlayer.ActorNumber);  // ���� ����� ���� ����
        }
        UpdateButtonsForRemainingPlayers();
    }

    // �÷��̾� ����Ʈ�� ������� UI�� ������Ʈ�ϴ� �Լ�
    void UpdatePlayerListUI()
    {
        for (int i = 0; i < playerTexts.Length; i++)
        {
            playerTexts[i].text = "";
            readylistText[i].text = "";
        }

        // �濡 �ִ� ��� �÷��̾��� �г����� �ؽ�Ʈ�� �߰�
        for (int i = 0; i < playerList.Count && i < playerTexts.Length; i++)
        {
            playerTexts[i].text = playerList[i].NickName;

            // �غ� ���¸� �ش� �÷��̾� ������ �°� ������Ʈ
            int playerIndex = i;
            if (playerReadyState.ContainsKey(playerList[i].ActorNumber))
            {
                bool isPlayerReady = playerReadyState[playerList[i].ActorNumber];
                readylistText[playerIndex].text = isPlayerReady ? "�غ� �Ϸ�" : "";
            }

            // ���õ� ��ư�� ������ �� ��ư ��Ȱ��ȭ
            if (playerSelectedButtonIndex.ContainsKey(playerList[i].ActorNumber))
            {
                int selectedButtonIndex = playerSelectedButtonIndex[playerList[i].ActorNumber];
                if (isConfirmed && selectedButtonIndex >= 0 && selectedButtonIndex < imageButtons.Length)
                {
                    // Ȯ�� ��ư�� �������� �� ��ư�� ��Ȱ��ȭ ���·� ǥ��
                    imageButtons[selectedButtonIndex].interactable = false;  // �ش� ��ư ��Ȱ��ȭ
                }
            }
        }
        CheckAllPlayersReady();
    }

    // ������ ������ �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        UpdatePlayerListUI(); // ���ο� ���� ���� ����
    }

    private void CheckAllPlayersReady()
    {
        bool allPlayersReady = true;

        // ��� �÷��̾ �غ� �Ϸ� �������� Ȯ��
        foreach (var playerState in playerReadyState)
        {
            if (!playerState.Value)
            {
                allPlayersReady = false;
                break;
            }
        }

        // ������ �غ� �Ϸ� ��ư�� ���� ���� ��ư���� ����
        if (allPlayersReady && PhotonNetwork.IsMasterClient)
        {
            gameStartBtn.gameObject.SetActive(true);  // ���� ���� ��ư Ȱ��ȭ
            readyBtn.gameObject.SetActive(false);  // �غ� ��ư ��Ȱ��ȭ
        }
        else
        {
            gameStartBtn.gameObject.SetActive(false);  // ���� ���� ��ư ��Ȱ��ȭ
            readyBtn.gameObject.SetActive(true);  // �غ� ��ư Ȱ��ȭ
        }
    }

    public void OnImageSwitchButtonClick(int imageIndex)
    {
        if (isConfirmed) return; // Ȯ���Ǿ����� �̹��� ������ ���ϰ� ����

        UpdateImage(imageIndex);
    }

    private void UpdateImage(int buttonIndex)
    {
        if (newSprites == null || newSprites.Length < 4) return;

        // ��ư Ŭ�� �� �̹����� ������Ʈ�մϴ�.
        if (targetImage != null && buttonIndex < newSprites.Length)
        {
            targetImage.sprite = newSprites[buttonIndex];  // �ش� �ε����� �̹����� Ÿ�� �̹����� ����
            playerSelectedButtonIndex[PhotonNetwork.LocalPlayer.ActorNumber] = buttonIndex;  // ������ ��ư �ε����� ����
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
        isConfirmed = true;  // Ȯ�� ��ư�� ������ ���� ��Ȱ��ȭ

        int selectedButtonIndex = playerSelectedButtonIndex[PhotonNetwork.LocalPlayer.ActorNumber];

        // Ȯ�� ��ư Ŭ�� �� ���õ� ��ư�� ��Ȱ��ȭ
        if (selectedButtonIndex >= 0 && selectedButtonIndex < imageButtons.Length)
        {
            imageButtons[selectedButtonIndex].interactable = false;  // �ش� ��ư�� ��Ȱ��ȭ�մϴ�.

            // �� ���¸� �ٸ� �÷��̾�鿡�� ����ȭ�մϴ�.
            photonView.RPC("SyncButtonDeactivation", RpcTarget.All, selectedButtonIndex);
        }

        readyBtn.interactable = true;
    }

    private void SyncButtonStatesWithNewPlayer(Photon.Realtime.Player newPlayer)
    {
        // �� �÷��̾ ������ ��ư�� ���� ���� �÷��̾�� ����
        foreach (var playerState in playerSelectedButtonIndex)
        {
            int playerActorNumber = playerState.Key;
            int selectedButtonIndex = playerState.Value;

            // ���õ� ��ư�� ��Ȱ��ȭ
            photonView.RPC("SyncButtonDeactivation", newPlayer, selectedButtonIndex);
        }
    }

    [PunRPC]
    public void SyncButtonDeactivation(int buttonIndex)
    {
        // ���õ� ��ư�� ��Ȱ��ȭ ���·� ����
        if (buttonIndex >= 0 && buttonIndex < imageButtons.Length)
        {
            imageButtons[buttonIndex].interactable = false;
        }
    }

    public void OnReadyButtonClick()
    {
        // �غ� ���� ���
        isReady = !isReady;

        // ���� �÷��̾��� �غ� ���¸� ������Ʈ
        playerReadyState[PhotonNetwork.LocalPlayer.ActorNumber] = isReady;

        // �غ� ���¿� ���� �ؽ�Ʈ�� ����
        if (ReadyText != null)
        {
            ReadyText.text = isReady ? "�غ� �Ϸ�" : "�غ�";
        }

        // UI ������Ʈ
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
            // ���� ����� ������ ��ư�� Ȱ��ȭ ���·� �����
            imageButtons[buttonIndex].interactable = true;
        }
    }

    // ���� ���� ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    private void OnGameStartButtonClick()
    {
        // ���� ���� ������ ���⿡ �߰�
        PhotonNetwork.LoadLevel("MultiplayScene"); // ����: "GameScene"�� �ε�
    }

    public void OnChangeNicknameButtonClick()
    {
        string newNickname = nicknameText.text;

        // �г����� ����Ǹ� ���� ��Ʈ��ũ�� �ݿ�
        if (!string.IsNullOrEmpty(newNickname) && newNickname != PhotonNetwork.NickName)
        {
            PhotonNetwork.NickName = newNickname;  // ���ο� �г����� ���� ��Ʈ��ũ�� ����
            UpdatePlayerListUI();  // �÷��̾� ��� UI ������Ʈ
            PlayerPrefs.SetString("Player", newNickname);

            photonView.RPC("UpdateNickname", RpcTarget.All, newNickname);
        }
    }

    [PunRPC]
    public void UpdateNickname(string newNickname)
    {
        // �г��� ������ ���� ��Ʈ��ũ�� �ݿ��Ǿ����Ƿ�, UI�� ����
        nicknameText.text = newNickname;  // UI���� �г��� ����
        UpdatePlayerListUI();  // �÷��̾� ��� UI ������Ʈ
    }

    private void UpdateButtonsForRemainingPlayers()
    {
        // ��� ��ư�� Ȱ��ȭ ���·� �ʱ�ȭ
        foreach (var button in imageButtons)
        {
            button.interactable = true;
        }

        // �����ִ� �÷��̾���� ���� ���¸� �ݿ��Ͽ� ��ư ��Ȱ��ȭ
        foreach (var playerState in playerSelectedButtonIndex)
        {
            int selectedButtonIndex = playerState.Value;

            // �̹� �ٸ� �÷��̾ ������ ��ư�� ��Ȱ��ȭ
            imageButtons[selectedButtonIndex].interactable = false;
        }
    }
}