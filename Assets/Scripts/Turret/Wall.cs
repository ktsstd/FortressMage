using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Wall : MonoBehaviourPunCallbacks, IPunObservable
{
    private AudioSource audioSource;
    public AudioClip audioClip;
    bool playEvent = false;

    public Image barImage;

    public float health;
    public float maxHealth;
    Animator animator;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;

        animator = transform.GetChild(0).gameObject.GetComponent<Animator>();

        maxHealth = 100f;
        health = maxHealth;
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

        if (barImage != null)
        {
            float healthPercentage = health / maxHealth;
            photonView.RPC("UpdateHealthBar", RpcTarget.AllBuffered, healthPercentage);
        }
    }

    [PunRPC]
    private void UpdateHealthBar(float healthPercentage)
    {
        barImage.fillAmount = healthPercentage;
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
