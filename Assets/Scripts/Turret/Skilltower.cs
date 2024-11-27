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
        // ������Ʈ ����
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
            
            // GameManager�� Wave ���� �����ͼ� ��
            float currentWave = GameManager.Instance.GetWave();
            GameManager.Instance.CheckTurretDestroyedOnWave(currentWave); // ���� ���̺� ����
        }
    }

    public void ResetHealth()
    {
        health = 100f; // ü���� 100���� ����     
    }
}
