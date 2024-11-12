using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public Vector3 targetPos;

    bool isDestroy;

    void Update()
    {
        if (targetPos != null && !isDestroy)
        {
            if (transform.position != targetPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, 6f * Time.deltaTime);
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(true);
                if (transform.GetChild(1).gameObject.activeSelf)
                    Invoke("DestroyFireBall", 2f);
                isDestroy = false;
            }
        }
        else
            return;
    }

    void DestroyFireBall()
    {
        Destroy(gameObject);
    }
}
