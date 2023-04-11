using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Player has Quit");
    }
}
