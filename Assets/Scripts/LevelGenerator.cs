using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public GameObject[] platformPrefabs;
    public float spawnDistance = 30.0f;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        System.Random rand = new System.Random();
        int platformIndex = rand.Next(0, platformPrefabs.Length);
        GameObject platformPrefab = platformPrefabs[platformIndex];
        if (other.gameObject == player)
        {
            Vector3 spawnPosition = transform.position + new Vector3(rand.Next(38, 43), rand.Next(-5, 5), 0);
            Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
