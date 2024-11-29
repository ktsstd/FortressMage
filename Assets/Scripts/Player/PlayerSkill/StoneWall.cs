using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneWall : MonoBehaviour
{
    public Quaternion oriRot;
    public Vector3 oriPos;
    void Start()
    {
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
