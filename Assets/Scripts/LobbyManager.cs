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

    private List<Photon.Realtime.Player> playerList = new List<Photon.Realtime.Player>();  // �濡 �ִ� �÷��̾� ����Ʈ

    public Image[] targetImage;  // ���� �̹��� ������Ʈ�� �迭�� ����

    public Sprite[] newSprites;  // ��ü�� �̹�����

    private bool isConfirmed = false; 

    void Awake()
    {
        exitBtn.onClick.AddListener(() => OnExitClick());
    }

    private void OnExitClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    // ���� �뿡�� �������� �� ȣ��Ǵ� �ݹ��Լ�
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("New Scene"); // ���� �� �� ��ȯ
    }

    void Start()
    {
        // �� �̸��� �ؽ�Ʈ�� ǥ��
        if (roomNameText != null)
        {
            roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        }

        // �÷��̾� ��Ͽ� ���� ���� ��� �÷��̾ �߰�
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            playerList.Add(player);
        }

        // UI ������Ʈ
        UpdatePlayerListUI();
    }

    // �濡 ���� �÷��̾ �����ϸ� �� �޼��尡 ȣ��˴ϴ�.
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        playerList.Add(newPlayer);
        UpdatePlayerListUI();


         if (messageText != null)
        {
            string msg = $"\n<color=#00ff00>{newPlayer.NickName}</color> ���� �濡 ���Խ��ϴ�!";
            messageText.text += msg; // �޽����� ��� �߰�
        }
    }

    // �濡�� �÷��̾ ������ �ؽ�Ʈ�� �ʱ�ȭ�� ���� �ֽ��ϴ�.
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        playerList.Remove(otherPlayer);
        UpdatePlayerListUI();


        if (messageText != null)
        {
            string msg = $"\n<color=#ff0000>{otherPlayer.NickName}</color> ���� ���� �������ϴ�.";
            messageText.text += msg; // �޽����� ��� �߰�
        }
    }

    // �÷��̾� ����Ʈ�� ������� UI�� ������Ʈ�ϴ� �Լ�
    void UpdatePlayerListUI()
    {
        // �÷��̾� �ؽ�Ʈ �迭�� ��� �ʱ�ȭ (�� �ؽ�Ʈ�� ����)
        for (int i = 0; i < playerTexts.Length; i++)
        {
            playerTexts[i].text = "";
        }

        // �濡 �ִ� ��� �÷��̾��� �г����� �ؽ�Ʈ�� �߰�
        for (int i = 0; i < playerList.Count && i < playerTexts.Length; i++)
        {
            playerTexts[i].text = playerList[i].NickName;
        }
    }

    // ������ ������ �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        UpdatePlayerListUI(); // ���ο� ���� ���� ����
    }

    public void OnImageSwitchButtonClick(int imageIndex)
    {
        if (isConfirmed) return; // Ȯ���Ǿ����� �̹��� ������ �� �� ������

        // ���� �÷��̾��� ��ȣ�� ���� ������Ʈ
        int targetPlayer = PhotonNetwork.LocalPlayer.ActorNumber;
        UpdateImageForPlayer(imageIndex, targetPlayer);
    }

    private void UpdateImageForPlayer(int buttonIndex, int playerNumber)
    {
        if (newSprites == null || newSprites.Length < 2 || playerNumber < 1 || playerNumber > 4) return;

        int startIndex = buttonIndex * 2;

        // 1�� Ÿ�� �̹����� ��� �÷��̾�� ����
        if (startIndex < newSprites.Length && targetImage[0] != null)
            targetImage[0].sprite = newSprites[startIndex];

        // �÷��̾ ���� Ÿ�� �̹��� ���� (playerNumber 1, 2, 3, 4�� ���� ���� 2��, 3��, 4��, 5�� Ÿ�� ����)
        int targetIndex = playerNumber;  // 1�� -> 1, 2�� -> 2, 3�� -> 3, 4�� -> 4

        if (targetIndex < targetImage.Length && startIndex + 1 < newSprites.Length && targetImage[targetIndex] != null)
            targetImage[targetIndex].sprite = newSprites[startIndex + 1];
    }

    public void OnConfirmButtonClick()
    {
        isConfirmed = true; // Ȯ�� ��ư�� ������ ĳ���� ������ ���ϰ� ����
    }
}