using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Skilltower : MonoBehaviourPun
{
    public float health = 100f;

    public GameObject objectToSpawn;
    public Transform spawnPoint;
    public float destroyDelay = 3f;
    public float damage;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpawnObject();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(damage);
        }
    }

    void SpawnObject()
    {
        // 오브젝트 생성
        if (objectToSpawn != null && spawnPoint != null)
        {
            GameObject spawnedObject = Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);
            Destroy(spawnedObject, destroyDelay);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            health = 0f;
            
            // GameManager의 Wave 값을 가져와서 비교
            float currentWave = GameManager.Instance.GetWave();
            GameManager.Instance.CheckTurretDestroyedOnWave(currentWave); // 현재 웨이브 전달
        }
    }

    public void ResetHealth()
    {
        health = 100f; // 체력을 100으로 리셋     
    }
}
