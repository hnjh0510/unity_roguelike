using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float dmg;

    void Start()
    {
        Destroy(gameObject, 10f);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Door"))
        {
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Enemy"))
        {
            // ������ �������� ����
            Enemy enemy = collision.GetComponent<Enemy>();
            Boss boss = collision.GetComponent<Boss>();
            if (boss != null)
            {
                Vector2 knockbackDirection = (boss.transform.position - transform.position).normalized;
                float knockbackForce = 2f; // �и��� �� ����
                boss.TakeDamage(dmg, knockbackDirection, knockbackForce);
            }
            else if (enemy != null)
            {
                enemy.TakeDamage(dmg);
            }
            Destroy(gameObject);
        }
    }
}
