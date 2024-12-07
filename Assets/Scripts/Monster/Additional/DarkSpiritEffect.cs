using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DarkSpiritEffect : MonoBehaviour
{
    private ParticleSystem particleSys;
    // Start is called before the first frame update
    void Start()
    {
        particleSys = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if(!particleSys.isPlaying)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
