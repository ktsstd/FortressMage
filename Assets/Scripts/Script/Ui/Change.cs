using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Change : MonoBehaviour
{
    public void Ingame()
    {
        SceneManager.LoadScene("Ingame");
    }
    public void Main()
    {
    	SceneManager.LoadScene("Main");
    }
}
