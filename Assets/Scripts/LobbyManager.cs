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

    private List<Photon.Realtime.Player> playerList = new List<Photon.Realtime.Player>();  // �濡 �ִ� �÷��̾� ����Ʈ


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
    }

    // �濡�� �÷��̾ ������ �ؽ�Ʈ�� �ʱ�ȭ�� ���� �ֽ��ϴ�.
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        playerList.Remove(otherPlayer);
        UpdatePlayerListUI();
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
}