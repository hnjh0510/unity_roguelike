using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 mousePos;
    private Camera mainCam;
    private Rigidbody2D rb;
    public float force;
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - transform.position;
        Vector3 rotation = transform.position - mousePos;
        rb.velocity = new Vector2(direction.x, direction.y).normalized * force;
        float rot = Mathf.Atan2(rotation.x, rotation.y * -1) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot-45);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Wall")||collision.CompareTag("Door"))
        {
            Destroy(gameObject);
        }
    }
}