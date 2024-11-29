using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneWall : MonoBehaviour
{

    void Start()
    {
        Invoke("SelfDestroy", 10f);
    }

    void Update()
    {
        gameObject.transform.GetChild(0).position = Vector3.MoveTowards(gameObject.transform.GetChild(0).position, transform.position, 5f * Time.deltaTime);
    }
    void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
