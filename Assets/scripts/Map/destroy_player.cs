using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroy_player : MonoBehaviour
{
    public GameObject player;  
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Destroy(player);
    }

    void Update()
    {
        
    }
}
