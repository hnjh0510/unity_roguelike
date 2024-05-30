using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyname;
    public float speed;
    public int health;

    public GameObject bullet;

    public float maxShotDelay;
    public float curShotDelay;

    public GameObject player;

    Animator anim;
    Rigidbody2D rigid;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        MoveTowardsPlayer();
        MoveAndShoot();
        Reload();
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
                    FireDirect(dirVec2D, 10);
                    break;
                case "B":
                    // Ÿ�� B�� �Ѿ��� �߻����� �ʰ� �ӵ� ����
                    rigid.velocity = dirVec2D * speed * 1.2f;
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
        int numberOfDirections = 12;
        float angleStep = 360f / numberOfDirections;
        for (int i = 0; i < numberOfDirections; i++)
        {
            float angle = i * angleStep;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, angle - 90));
            Rigidbody2D bulletRigidbody = newBullet.GetComponent<Rigidbody2D>();
            bulletRigidbody.velocity = direction * 5;
        }
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    public void OnHit(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Destroy(gameObject); // �� ��� ó��
        }
    }
}
