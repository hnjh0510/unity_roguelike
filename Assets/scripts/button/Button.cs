using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public void GameStartNextScene()
    {
        SceneManager.LoadScene(1);
        Player.isInitialized = false;
    }

    public void ToStartScene()
    {
        SceneManager.LoadScene(0);
    }
}
