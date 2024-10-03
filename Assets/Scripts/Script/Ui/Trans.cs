using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trans : MonoBehaviour
{
    public Button menuButton; 

	public void OnClickButton()
    {
        Vector3 currentPosition = transform.position;
        transform.position = new Vector3(currentPosition.x, 229, currentPosition.z);
        menuButton.interactable = false;
    }
    
    public void OnClickButtons()
    {
        Vector3 currentPosition = transform.position;
        transform.position = new Vector3(currentPosition.x, 217, currentPosition.z);
        menuButton.interactable = true;
    }
}
