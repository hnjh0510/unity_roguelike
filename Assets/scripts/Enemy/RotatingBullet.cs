using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingBullet : MonoBehaviour
{
    public float speed = 2f;
    public float rotationSpeed;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rb.velocity = Quaternion.Euler(0, 0, rotationSpeed * Time.deltaTime) * rb.velocity;
    }
}
