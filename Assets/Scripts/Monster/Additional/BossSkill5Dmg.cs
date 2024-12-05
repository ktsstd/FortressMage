using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BossSkill5Dmg : MonoBehaviour
{
    public GameObject childObj;
    public GameObject DmgObj;
    private Vector3 Pos1;
    private Vector3 thisPos;
    private ParticleSystem particleSystem;
    private ParticleSystem thisparticleSystem;
    private bool isDmg = false;

    void Start()
    {
        thisPos = transform.position;
        Pos1 = new Vector3(thisPos.x, thisPos.y - 50, thisPos.z);
        particleSystem = childObj.GetComponent<ParticleSystem>();
        thisparticleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!particleSystem.isPlaying)
        {
            PhotonNetwork.Destroy(gameObject);
        }

        if (!thisparticleSystem.isPlaying)
        {
            if (isDmg) return;           
            Instantiate(DmgObj, Pos1, Quaternion.identity);
            isDmg = true;
        }
    }
}
