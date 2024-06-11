using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroy_player : MonoBehaviour
{
    public GameObject player;
    public GameObject pb;
    public GameObject es;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pb = GameObject.FindGameObjectWithTag("HpBar");
        es = GameObject.FindGameObjectWithTag("es");
        Destroy(player);
        Destroy(pb);
        Destroy(es);
    }

    void Update()
    {
        
    }
}
