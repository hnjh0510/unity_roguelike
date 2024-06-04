using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Bullet : MonoBehaviour
{
    public float dmg;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Door"))
        {
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Player"))
        {
            // 플레이어에게 데미지를 입힘
            Player player = collision.GetComponent<Player>();
            if (player != null && player.isInvincible == false)
            {
                player.TakeDamage(dmg);
                Debug.Log("체력감소 현재체력:" + Player.health);
            }
            Destroy(gameObject);
        }
    }
}
