using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public float dmg;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(dmg);
            }
        }
        else if (collision.CompareTag("Boss"))
        {
            Boss boss = collision.GetComponent<Boss>();
            if (boss != null)
            {
                boss.TakeDamage(dmg);
            }
        }
    }
}
