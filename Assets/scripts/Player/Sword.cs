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
            Boss boss = collision.GetComponent<Boss>();
            if (boss != null)
            {
                Vector2 knockbackDirection = (boss.transform.position - transform.position).normalized;
                float knockbackForce = 2f; // 밀리는 힘 설정
                boss.TakeDamage(dmg, knockbackDirection, knockbackForce);
            }
            else if (enemy != null)
            {
                enemy.TakeDamage(dmg);
            }
        }
    }
}
