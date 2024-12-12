using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Wall : MonoBehaviourPunCallbacks, IPunObservable
{
    private AudioSource audioSource;
    public AudioClip audioClip;
    bool playEvent = false;

    public float health = 100f;
    Animator animator;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;

        animator = transform.GetChild(0).gameObject.GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0f && !playEvent)
        {
            photonView.RPC("OnDestroyWall", RpcTarget.All);
            audioSource.PlayOneShot(audioClip, 0.5f);
            playEvent = true;
        }
    }

    [PunRPC]
    public void OnDestroyWall()
    {
        animator.SetTrigger("Defeat");
        GameManager.Instance.StartCoroutine("DefeatEvent");
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (float)stream.ReceiveNext();
        }
    }
}
