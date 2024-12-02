using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImg : MonoBehaviour

{
    // 변경될 이미지를 표시할 Image 컴포넌트
    public Image targetImage;

    // 버튼 클릭 시 변경될 이미지들
    public Sprite[] newSprites;

    // 버튼 클릭 시 이미지를 변경하는 함수
    public void OnButtonClick(int index)
    {
        // index 값에 해당하는 이미지로 변경
        if (targetImage != null && newSprites != null && newSprites.Length > index)
        {
            targetImage.sprite = newSprites[index];
        }
    }
}

