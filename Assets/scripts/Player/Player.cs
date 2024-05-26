using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rigid;
    public Animator pAni;
    

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        pAni = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        rigid.velocity = new Vector3(x, y, 0).normalized * speed;
        if (x != 0 || y != 0)
            pAni.SetBool("Move", true);
        else
            pAni.SetBool("Move", false);
    }
}
