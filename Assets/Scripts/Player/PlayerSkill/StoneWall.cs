using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneWall : MonoBehaviour
{
    Quaternion oriRot;
    void Start()
    {
        oriRot = transform.GetChild(0).rotation;
        Invoke("SelfDestroy", 10f);
    }

    void Update()
    {
        transform.GetChild(0).position = Vector3.MoveTowards(transform.GetChild(0).position, transform.position, 5f * Time.deltaTime);
        transform.GetChild(0).rotation = oriRot;
    }
    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
