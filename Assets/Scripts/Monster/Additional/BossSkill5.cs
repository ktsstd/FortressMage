using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill5 : MonoBehaviour
{
    private Vector3 defaultPos;
    void Start()
    {
        defaultPos = transform.position;
        GameObject[] playertag = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject playerObj in playertag)
        {
            playerObj.transform.position = defaultPos;
        }
    }
}
