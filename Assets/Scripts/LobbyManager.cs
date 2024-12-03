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
    public Button confirmBtn;
    private bool isFirstImageUpdated = false;


    void Awake()
    {
        exitBtn.onClick.AddListener(() => OnExitClick());
        confirmBtn.onClick.AddListener(() => OnConfirmClick());  // Ȯ�� ��ư Ŭ�� �� ȣ��� �Լ� ���
    }
    private void OnConfirmClick()
{
    // �� ��° �̹����� �����ϴ� �Լ� ȣ��
    UpdateSecondImageLocally();
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
    // ��ư Ŭ�� �� ȣ��Ǵ� �Լ� (�̹����� �����ϴ� �Լ�)
    public void OnImageSwitchButtonClick()
    {
        // �̹����� �����ϴ� �Լ� ȣ��
        UpdateImagesLocally();
    }

    private void UpdateImagesLocally()
{
    if (newSprites != null && newSprites.Length > 0)
    {
        if (targetImage.Length > 0 && targetImage[0] != null)
        {
            targetImage[0].sprite = newSprites[0];
            isFirstImageUpdated = true;  // ù ��° �̹����� ����Ǿ����� ǥ��
        }
    }
}
private void UpdateSecondImageLocally()
{
    // ù ��° �̹����� ����� ��, �� ��° �̹����� ����
    if (isFirstImageUpdated && newSprites != null && newSprites.Length > 1)
    {
        if (targetImage.Length > 1 && targetImage[1] != null)
        {
            targetImage[1].sprite = newSprites[1];
        }
    }
}
}