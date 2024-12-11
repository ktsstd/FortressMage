using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneWall : MonoBehaviour
{
    public Quaternion oriRot;
    public Vector3 oriPos;

    private AudioSource audioSource;
    public AudioClip audioClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;

        Invoke("SelfDestroy", 10f);

        audioSource.PlayOneShot(audioClip, 0.3f);
    }

    void Update()
    {
        transform.GetChild(0).position = Vector3.MoveTowards(transform.GetChild(0).position, transform.position + Vector3.down * 2, 5f * Time.deltaTime);
        transform.rotation = oriRot;
        transform.position = oriPos;
    }
    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
