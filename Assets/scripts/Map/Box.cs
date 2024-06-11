using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public GameObject player;
    public bool isOpen = false;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == player && isOpen == false)
        {
            //���� ���
            Debug.Log("���� ���");
            isOpen = true;
        }
    }
}
