using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{ 
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Wall")||collision.CompareTag("Door"))
        {
            Destroy(gameObject);
        }
    }
}