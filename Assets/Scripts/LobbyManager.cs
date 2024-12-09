using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;

public class LobbyManager : MonoBehaviourPunCallbacks

{
    public TMP_Text roomNameText;
    public TMP_Text[] playerTexts;
    public TMP_Text[] warningTexts;
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
    private bool isFading = false;

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
        // 본인 준비 상태 초기화
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "isReady", false }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        // 기존 플레이어들의 준비 상태 UI 업데이트
        UpdateAllReadyStates();
    }

    void UpdateAllReadyStates()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            bool isPlayerReady = player.CustomProperties.ContainsKey("isReady") && (bool)player.CustomProperties["isReady"];
            Debug.Log($"{player.NickName} 준비 상태: {isPlayerReady}");
            
            // UI 업데이트
            int index = playerList.IndexOf(player); // 플레이어 UI의 인덱스 계산
            if (index >= 0 && index < readylistText.Length)
            {
                readylistText[index].text = isPlayerReady ? "준비완료" : "";
            }
        }
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
                readylistText[playerIndex].text = isPlayerReady ? "준비완료" : "";
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
    public override void OnMasterClientSwitched(Player newMasterClient)
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

       if (targetImage != null && buttonIndex < newSprites.Length)
    {
        targetImage.sprite = newSprites[buttonIndex];  // �ش� �ε����� �̹����� Ÿ�� �̹����� ����
        playerSelectedButtonIndex[PhotonNetwork.LocalPlayer.ActorNumber] = buttonIndex;  // ������ ��ư �ε����� ����

        // ���õ� ĳ���� ������ PhotonNetwork.LocalPlayer�� CustomProperties�� ����
        ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable();
        playerProps.Add("selectedCharacter", buttonIndex);  // ���õ� ĳ������ �ε����� ����
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
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

        int selectedButtonIndex = playerSelectedButtonIndex.ContainsKey(PhotonNetwork.LocalPlayer.ActorNumber) 
                                ? playerSelectedButtonIndex[PhotonNetwork.LocalPlayer.ActorNumber] 
                                : -1;

        // Ȯ�� ��ư Ŭ�� �� ���õ� ��ư�� ��Ȱ��ȭ
        if (selectedButtonIndex >= 0 && selectedButtonIndex < imageButtons.Length)
        {
            imageButtons[selectedButtonIndex].interactable = false;  // �ش� ��ư�� ��Ȱ��ȭ�մϴ�.

            // �� ���¸� �ٸ� �÷��̾�鿡�� ����ȭ�մϴ�.
            photonView.RPC("SyncButtonDeactivation", RpcTarget.All, selectedButtonIndex);
            
        }
        readyBtn.interactable = true;
        if (selectedButtonIndex == -1)
        {
            isConfirmed = false;
            readyBtn.interactable = false;
            StartCoroutine(FadeInOut1());
        }
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

        // 준비 상태를 Custom Properties에 업데이트
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "isReady", isReady }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        // ���� �÷��̾��� �غ� ���¸� ������Ʈ
        playerReadyState[PhotonNetwork.LocalPlayer.ActorNumber] = isReady;

        // �غ� ���¿� ���� �ؽ�Ʈ�� ����
        if (ReadyText != null)
        {
            ReadyText.text = isReady ? "준비완료" : "준비";
        }

        // UI ������Ʈ
        UpdatePlayerListUI();

        photonView.RPC("UpdateReadyState", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, isReady);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("isReady"))
        {
            UpdateAllReadyStates(); // 상태를 UI에 반영
        }
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
        // asdfasdffffffffff
        // asdfffffffffff
        // fdasfasdfffffffffffff
    private bool isStarting = false;
    // ���� ���� ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    private void OnGameStartButtonClick()
    {
        if (!isStarting)
        {
            PhotonNetwork.LoadLevel("MultiplayScene");
            isStarting = true;
        }
        else
        {
            StartCoroutine(FadeInOut2());
        }
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

    private IEnumerator FadeInOut1()
    {
        if(isFading) yield break;
        isFading = true;
        float fadevalue = 0f;
        float fadereturnvalue = 0.6f;
        Color origColor = warningTexts[0].color;
        while(fadevalue < fadereturnvalue)
        {
            float alphaValue = Mathf.Lerp(0f, 1f, fadevalue / fadereturnvalue);
            warningTexts[0].color = new Color(origColor.r, origColor.g, origColor.b, alphaValue);
            fadevalue += Time.deltaTime;
            yield return null;
        }
        warningTexts[0].color = new Color(origColor.r, origColor.g, origColor.b, 1f);

        yield return new WaitForSeconds(1f);

        fadevalue = 0f;
        while (fadevalue < fadereturnvalue)
        {
            float alphaValue = Mathf.Lerp(1f, 0f, fadevalue / fadereturnvalue);
            warningTexts[0].color = new Color(origColor.r, origColor.g, origColor.b, alphaValue);
            fadevalue += Time.deltaTime;
            yield return null;
        }
        warningTexts[0].color = new Color(origColor.r, origColor.g, origColor.b, 0f);
        isFading = false;
    }

    private IEnumerator FadeInOut2()
    {
        if(isFading) yield break;
        isFading = true;
        float fadevalue = 0f;
        float fadereturnvalue = 0.6f;
        Color origColor = warningTexts[0].color;
        while(fadevalue < fadereturnvalue)
        {
            float alphaValue = Mathf.Lerp(0f, 1f, fadevalue / fadereturnvalue);
            warningTexts[0].color = new Color(origColor.r, origColor.g, origColor.b, alphaValue);
            fadevalue += Time.deltaTime;
            yield return null;
        }
        warningTexts[0].color = new Color(origColor.r, origColor.g, origColor.b, 1f);

        yield return new WaitForSeconds(1f);

        fadevalue = 0f;
        while (fadevalue < fadereturnvalue)
        {
            float alphaValue = Mathf.Lerp(1f, 0f, fadevalue / fadereturnvalue);
            warningTexts[0].color = new Color(origColor.r, origColor.g, origColor.b, alphaValue);
            fadevalue += Time.deltaTime;
            yield return null;
        }
        warningTexts[0].color = new Color(origColor.r, origColor.g, origColor.b, 0f);
        isFading = false;
    }
}