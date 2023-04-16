using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectCoin : MonoBehaviour
{
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            if(gameObject.tag == "RedCoin")
            {
                ScoreManager.Instance.AddScore(5);
            } else
            {
                ScoreManager.Instance.AddScore(3);
            }
            Destroy(gameObject);
        }
    }
}
