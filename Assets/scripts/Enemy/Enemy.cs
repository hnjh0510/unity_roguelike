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
    public GameObject[] magicItems; // ���� ������ ������ �迭

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
        // ������ ��� Ȯ��
        float dropChance = 0.1f;
        if (Random.value <= dropChance)
        {
            // �������� ������ ����
            int randomIndex = Random.Range(0, magicItems.Length);
            Instantiate(magicItems[randomIndex], transform.position, Quaternion.identity);
        }

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
                        // �ѹ� ����
                        FireDirect(dirVec2D, 2f);
                        break;
                    case "B":
                        // Ÿ�� B�� �Ѿ��� �߻����� �ʰ� �ӵ� ����
                        rigid.velocity = dirVec2D * speed * 1.5f;
                        break;
                    case "C":
                        // �翷���� �߻�
                        FireRightandLeft();
                        break;
                    case "D":
                        //�ι� �߻�
                        FireDirect2(dirVec2D, 2.5f);
                        break;
                    case "E":
                        // �����԰��鼭 ���� �߻�
                        rigid.velocity = dirVec2D * speed * 1.7f;
                        FireDirect(dirVec2D, 4);
                        break;
                    case "F":
                        //4���� �߻�
                        FireInFourDirections();
                        break;
                    case "G":
                        //���� �߻�
                        FireDirect3(dirVec2D, 3f);
                        break;
                    case "H":
                        //Ŀ���鼭 ����(�����)
                        StartCoroutine(Boom());
                        break;
                    case "I":
                        // 4�������� �����鼭 ȸ�� 10�ʵ� �����
                        FireInFourDirections2();
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

    void FireDirect2(Vector2 direction, float force)
    {
        Vector3 rightOffset = new Vector3(0.2f, 0, 0); // ������
        Vector3 leftOffset = new Vector3(-0.2f, 0, 0); // ����

        // ������ �Ѿ� �߻�
        GameObject rightBullet = Instantiate(bullet, transform.position + rightOffset, Quaternion.identity);
        Rigidbody2D rightBulletRigidbody = rightBullet.GetComponent<Rigidbody2D>();
        rightBulletRigidbody.AddForce(direction * force, ForceMode2D.Impulse);

        // ���� �Ѿ� �߻�
        GameObject leftBullet = Instantiate(bullet, transform.position + leftOffset, Quaternion.identity);
        Rigidbody2D leftBulletRigidbody = leftBullet.GetComponent<Rigidbody2D>();
        leftBulletRigidbody.AddForce(direction * force, ForceMode2D.Impulse);
    }

    void FireDirect3(Vector2 direction, float force)
    {
        Vector3 rightOffset = new Vector3(0.2f, 0, 0); // ������
        Vector3 leftOffset = new Vector3(-0.2f, 0, 0); // ����

        // �߰� �Ѿ� �߻�
        GameObject middleBullet = Instantiate(bullet, transform.position, Quaternion.identity);
        Rigidbody2D middleBulletRigidbody = middleBullet.GetComponent<Rigidbody2D>();
        middleBulletRigidbody.AddForce(direction * force, ForceMode2D.Impulse);

        // ���� ���
        float angle = 15f * Mathf.Deg2Rad;

        // ������ �Ѿ� �߻�
        Vector2 rightDirection = new Vector2(
            direction.x * Mathf.Cos(angle) - direction.y * Mathf.Sin(angle),
            direction.x * Mathf.Sin(angle) + direction.y * Mathf.Cos(angle)
        );
        GameObject rightBullet = Instantiate(bullet, transform.position + rightOffset, Quaternion.identity);
        Rigidbody2D rightBulletRigidbody = rightBullet.GetComponent<Rigidbody2D>();
        rightBulletRigidbody.AddForce(rightDirection * force, ForceMode2D.Impulse);

        // ���� �Ѿ� �߻�
        Vector2 leftDirection = new Vector2(
            direction.x * Mathf.Cos(-angle) - direction.y * Mathf.Sin(-angle),
            direction.x * Mathf.Sin(-angle) + direction.y * Mathf.Cos(-angle)
        );
        GameObject leftBullet = Instantiate(bullet, transform.position + leftOffset, Quaternion.identity);
        Rigidbody2D leftBulletRigidbody = leftBullet.GetComponent<Rigidbody2D>();
        leftBulletRigidbody.AddForce(leftDirection * force, ForceMode2D.Impulse);
    }

    void FireRightandLeft()
    {
        int numberOfDirections = 2;
        float angleStep = 360f / numberOfDirections;
        for (int i = 0; i < numberOfDirections; i++)
        {
            float angle = i * angleStep;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, angle));
            Rigidbody2D bulletRigidbody = newBullet.GetComponent<Rigidbody2D>();
            bulletRigidbody.velocity = direction * 2f;
        }
    }
    void FireInFourDirections()
    {
        Vector2[] directions = new Vector2[]
        {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
        };

        foreach (Vector2 direction in directions)
        {
            GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.identity);
            Rigidbody2D bulletRigidbody = newBullet.GetComponent<Rigidbody2D>();
            bulletRigidbody.velocity = direction * 2.5f;
        }
    }

    void FireInFourDirections2()
    {
        // �� ���� ���� ���͸� �����մϴ�: ��, ��, ��, ��
        Vector2[] directions = new Vector2[]
        {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        for (int i = 0; i < directions.Length; i++)
        {
            GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.identity);
            Rigidbody2D bulletRigidbody = newBullet.GetComponent<Rigidbody2D>();

            //�Ѿ� �ʱ� �ӵ� ����
            bulletRigidbody.velocity = directions[i] * 3f;

            RotatingBullet rotatingBullet = newBullet.AddComponent<RotatingBullet>();
            rotatingBullet.speed = 3; // �ʱ� �ӵ� ����
            rotatingBullet.rotationSpeed = 60f; // ȸ�� �ӵ� ����
        }
    }
    IEnumerator Boom()
    {
        // ũ�⸦ ���������� ������Ű��
        float scaleIncreaseDuration = 1f; // ũ�Ⱑ �����ϴ� �ð�
        Vector3 initialScale = transform.localScale;
        Vector3 finalScale = initialScale * 2; // �� �� ũ��� ����

        float elapsed = 0f;
        while (elapsed < scaleIncreaseDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, finalScale, elapsed / scaleIncreaseDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // ���� ũ��� ����
        transform.localScale = finalScale;

        Destroy(gameObject);
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
