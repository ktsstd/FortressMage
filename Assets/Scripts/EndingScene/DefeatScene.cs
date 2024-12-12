using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DefeatScene : MonoBehaviour
{
    public Image storyImage;
    public Sprite[] storyImages;
    public TextMeshProUGUI storyText;

    int num = 0;

    public void ClickEvent()
    {
        num++;
        switch (num)
        {
            case 1:
                storyImage.sprite = storyImages[0];
                storyText.text = "제국의 경계에서 시작된 타락한 정령들의 습격은 점점 거세지며, 사람들은 두려움에 떨고 있었다.이들은 제국의 번영의 원천이 된 자원과 사람들의 목숨을 빼앗고, 나아가 세상을 어둠으로 지배하려는 존재들이었다.이 타락한 정령들을 부리는 자는 과거에 금기 마법인 흑마법에 손을 대어 제국에서 추방당한 이었고, 스스로를 어둠의 군주라 칭하였다";
                break;
            case 2:
                storyImage.sprite = storyImages[1];
                storyText.text = "황제는 이 참담한 상황을 해결하기 위해 제국 최고의 마법사들에게 도움을 요청했다. \"우리의 성벽을 넘으려는 자들로부터 우리의 땅과 백성들을 지켜주게나!\"황제의 목소리는 절박함에 차 있었고, 마법사들은 제국이 암흑기에 처해 있다는 사실을 외면할 수 없었다.마법사들의 결의가 필요한 순간이었다.";
                break;
            case 3:
                storyImage.sprite = storyImages[2];
                storyText.text = "밝은 낮의 태양 아래, 제국의 성벽 위에 선택받은 마법사들이 모였다.그들은 각기 다른 원소의 힘을 지닌 마법사들로 화염, 얼음, 전기, 그리고 대지의 마법을 다루었다.그들은 성벽을 향해 몰려오는 타락한 정령들을 바라보며 외쳤다.“우리는 제국의 운명을 지키기 위해 여기에 모였으며 그들의 침략을 막아야 한다! 어둠에 저항하라!”";
                break;
        }
    }

    public void MoveScene()
    {
        SceneManager.LoadScene("New Scene");
    }
}
