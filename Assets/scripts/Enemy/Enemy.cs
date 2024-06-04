using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyname;
    public float speed;
    public float health;

    public GameObject bullet;

    public float maxShotDelay;
    public float curShotDelay;

    public GameObject player;

    Animator anim;
    Rigidbody2D rigid;

    public bool toMove;
    public GameObject isInside;
    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        // ���� �±׸� ���� ������Ʈ�� ���� �浹�� ����
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objectsWithTag)
        {
            if (obj != this.gameObject)
            {
                Collider2D collider1 = obj.GetComponent<Collider2D>();
                Collider2D collider2 = GetComponent<Collider2D>();
                if (collider1 != null && collider2 != null)
                {
                    Physics2D.IgnoreCollision(collider1, collider2);
                }
            }
        }
    }

    void Update()
    {
        if (isInside != null)
        {
            IsInside tomove = isInside.GetComponent<IsInside>();
            if (tomove != null)
            {
                toMove = tomove.emove;
            }
        }
        if (toMove)
        {
            MoveTowardsPlayer();
            MoveAndShoot();
            Reload();
        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        // ���� �׾��� ���� ����
        Destroy(gameObject);
    }
    void MoveTowardsPlayer()
    {
        // �÷��̾ ���� ���� ���
        Vector3 dirVec = player.transform.position - transform.position;
        Vector2 dirVec2D = new Vector2(dirVec.x, dirVec.y).normalized;

        // ���� �ӵ� ����
        rigid.velocity = dirVec2D * speed;

        // �ִϸ��̼� ���� ������Ʈ
        anim.SetBool("E_move", rigid.velocity != Vector2.zero);

        // ���� ��������Ʈ ���� ����
        if (dirVec2D.x != 0)
        {
            // localScale.x�� dirVec2D.x�� ��ȣ�� ���� �����Ͽ� �����̳� �������� �ٶ󺸰� ��
            transform.localScale = new Vector3(Mathf.Sign(dirVec2D.x)*(0.15f), 0.15f, 0.15f);
        }
    }

    void MoveAndShoot()
    {
        if (curShotDelay >= maxShotDelay)
        {
            Vector3 dirVec = player.transform.position - transform.position;
            Vector2 dirVec2D = new Vector2(dirVec.x, dirVec.y).normalized;
            switch (enemyname)
            {
                case "A":
                    // Ÿ�� A�� ���� ����
                    FireDirect(dirVec2D, 3);
                    break;
                case "B":
                    // Ÿ�� B�� �Ѿ��� �߻����� �ʰ� �ӵ� ����
                    rigid.velocity = dirVec2D * speed * 2f;
                    break;
                case "C":
                    // Ÿ�� C�� ���� ����
                    FireCircular();
                    break;
            }

            curShotDelay = 0;
        }
    }

    void FireDirect(Vector2 direction, float force)
    {
        GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.identity);
        Rigidbody2D bulletRigidbody = newBullet.GetComponent<Rigidbody2D>();
        bulletRigidbody.AddForce(direction * force, ForceMode2D.Impulse);
    }

    void FireCircular()
    {
        int numberOfDirections = 2;
        float angleStep = 360f / numberOfDirections;
        for (int i = 0; i < numberOfDirections; i++)
        {
            float angle = i * angleStep;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, angle));
            Rigidbody2D bulletRigidbody = newBullet.GetComponent<Rigidbody2D>();
            bulletRigidbody.velocity = direction * 3f;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (collision.gameObject.CompareTag("Player") && player != null && player.isInvincible == false)
        {
            player.TakeDamage(1);
            Debug.Log("ü�°��� ����ü��:" + Player.health);
        }
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isInside = collision.gameObject;
    }
}
