using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DarkSpirit : MonoBehaviour
{
    private Animator animator;
    private Transform skilltowerTransform;

     void Start()
    {
        animator = GetComponent<Animator>();
        GameObject skilltowerObject = GameObject.FindWithTag("skilltower");
        if (castleObject != null)
        {
            skilltowerTransform = skilltowerObject.transform;
        }
    }
}
