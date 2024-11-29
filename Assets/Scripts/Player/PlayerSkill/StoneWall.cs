using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneWall : MonoBehaviour
{
    Quaternion oriRot;
    Vector3 oriPos;
    void Start()
    {
        oriRot = transform.rotation;
        oriPos = transform.position;
        Invoke("SelfDestroy", 10f);
    }

    void Update()
    {
        transform.GetChild(0).position = Vector3.MoveTowards(transform.GetChild(0).position, transform.position, 5f * Time.deltaTime);
        transform.rotation = oriRot;
        transform.position = oriPos;
    }
    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
