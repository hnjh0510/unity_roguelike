using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Potal : MonoBehaviour
{
    public GameObject player;
    public int sceneCount;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        sceneCount = SceneManager.GetActiveScene().buildIndex;
    }
    private void OnTriggerEnter2D(Collider2D collision)//다음씬 넘어가기
    {
        if (collision.tag == "Player")
        {
            sceneCount++;
            SceneManager.LoadScene(sceneCount);
        }
    }
}
