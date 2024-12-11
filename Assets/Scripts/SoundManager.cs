using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] MonsterAudio; 
    private AudioSource audioSources; 

    void Start()
    {
        audioSources = gameObject.AddComponent<AudioSource>();
    }

    public void PlayMonster(int monsterIndex, float volume, Vector3 position)
    {
        if (monsterIndex < 0 || monsterIndex >= MonsterAudio.Length)
        {
            return;
        }

        AudioSource.PlayClipAtPoint(MonsterAudio[monsterIndex], position, volume);
    }
}
