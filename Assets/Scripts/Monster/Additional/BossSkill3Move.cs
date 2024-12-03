using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill3Move : MonoBehaviour
{
    private float movespeed = 4f;
    private Transform CastleObjPos;
    // Start is called before the first frame update
    void Start()
    {
        GameObject castleObj = GameObject.FindWithTag("Castle");
        CastleObjPos = castleObj.transform;
        // transform.position = Vector3.MoveTowards(transform.position, CastleObjPos.position, movespeed);
    }

    void Update()
    {
        if (transform.position == CastleObjPos.position)
        {
            Destroy(gameObject);
        }
        transform.position = Vector3.MoveTowards(transform.position, CastleObjPos.position, movespeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerScript = other.GetComponent<PlayerController>();
            playerScript.OnPlayerBlind();
        }        
    }
}
