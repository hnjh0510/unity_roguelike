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
    public GameObject[] magicItems; // 매직 아이템 프리팹 배열

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

        // 같은 태그를 가진 오브젝트들 간의 충돌을 무시
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
        // 아이템 드랍 확률
        float dropChance = 0.1f;
        if (Random.value <= dropChance)
        {
            // 랜덤으로 아이템 선택
            int randomIndex = Random.Range(0, magicItems.Length);
            Instantiate(magicItems[randomIndex], transform.position, Quaternion.identity);
        }

        // 적이 죽었을 때의 로직
        Destroy(gameObject);
    }
    void MoveTowardsPlayer()
    {
        // 플레이어를 향한 방향 계산
        Vector3 dirVec = player.transform.position - transform.position;
        Vector2 dirVec2D = new Vector2(dirVec.x, dirVec.y).normalized;

        // 적의 속도 설정
        rigid.velocity = dirVec2D * speed;

        // 애니메이션 상태 업데이트
        anim.SetBool("E_move", rigid.velocity != Vector2.zero);

        // 적의 스프라이트 방향 조정
        if (dirVec2D.x != 0)
        {
            // localScale.x를 dirVec2D.x의 부호에 따라 조정하여 왼쪽이나 오른쪽을 바라보게 함
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
                        // 한발 공격
                        FireDirect(dirVec2D, 2f);
                        break;
                    case "B":
                        // 타입 B는 총알을 발사하지 않고 속도 증가
                        rigid.velocity = dirVec2D * speed * 1.5f;
                        break;
                    case "C":
                        // 양옆으로 발사
                        FireRightandLeft();
                        break;
                    case "D":
                        //두발 발사
                        FireDirect2(dirVec2D, 2.5f);
                        break;
                    case "E":
                        // 빠르게가면서 공격 발사
                        rigid.velocity = dirVec2D * speed * 1.7f;
                        FireDirect(dirVec2D, 4);
                        break;
                    case "F":
                        //4방향 발사
                        FireInFourDirections();
                        break;
                    case "G":
                        //세발 발사
                        FireDirect3(dirVec2D, 3f);
                        break;
                    case "H":
                        //커지면서 터짐(사라짐)
                        StartCoroutine(Boom());
                        break;
                    case "I":
                        // 4방향으로 나가면서 회전 10초뒤 사라짐
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
        Vector3 rightOffset = new Vector3(0.2f, 0, 0); // 오른쪽
        Vector3 leftOffset = new Vector3(-0.2f, 0, 0); // 왼쪽

        // 오른쪽 총알 발사
        GameObject rightBullet = Instantiate(bullet, transform.position + rightOffset, Quaternion.identity);
        Rigidbody2D rightBulletRigidbody = rightBullet.GetComponent<Rigidbody2D>();
        rightBulletRigidbody.AddForce(direction * force, ForceMode2D.Impulse);

        // 왼쪽 총알 발사
        GameObject leftBullet = Instantiate(bullet, transform.position + leftOffset, Quaternion.identity);
        Rigidbody2D leftBulletRigidbody = leftBullet.GetComponent<Rigidbody2D>();
        leftBulletRigidbody.AddForce(direction * force, ForceMode2D.Impulse);
    }

    void FireDirect3(Vector2 direction, float force)
    {
        Vector3 rightOffset = new Vector3(0.2f, 0, 0); // 오른쪽
        Vector3 leftOffset = new Vector3(-0.2f, 0, 0); // 왼쪽

        // 중간 총알 발사
        GameObject middleBullet = Instantiate(bullet, transform.position, Quaternion.identity);
        Rigidbody2D middleBulletRigidbody = middleBullet.GetComponent<Rigidbody2D>();
        middleBulletRigidbody.AddForce(direction * force, ForceMode2D.Impulse);

        // 각도 계산
        float angle = 15f * Mathf.Deg2Rad;

        // 오른쪽 총알 발사
        Vector2 rightDirection = new Vector2(
            direction.x * Mathf.Cos(angle) - direction.y * Mathf.Sin(angle),
            direction.x * Mathf.Sin(angle) + direction.y * Mathf.Cos(angle)
        );
        GameObject rightBullet = Instantiate(bullet, transform.position + rightOffset, Quaternion.identity);
        Rigidbody2D rightBulletRigidbody = rightBullet.GetComponent<Rigidbody2D>();
        rightBulletRigidbody.AddForce(rightDirection * force, ForceMode2D.Impulse);

        // 왼쪽 총알 발사
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
        // 네 개의 방향 벡터를 정의합니다: 상, 하, 좌, 우
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

            //총알 초기 속도 설정
            bulletRigidbody.velocity = directions[i] * 3f;

            RotatingBullet rotatingBullet = newBullet.AddComponent<RotatingBullet>();
            rotatingBullet.speed = 3; // 초기 속도 설정
            rotatingBullet.rotationSpeed = 60f; // 회전 속도 설정
        }
    }
    IEnumerator Boom()
    {
        // 크기를 점진적으로 증가시키기
        float scaleIncreaseDuration = 1f; // 크기가 증가하는 시간
        Vector3 initialScale = transform.localScale;
        Vector3 finalScale = initialScale * 2; // 두 배 크기로 증가

        float elapsed = 0f;
        while (elapsed < scaleIncreaseDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, finalScale, elapsed / scaleIncreaseDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 최종 크기로 설정
        transform.localScale = finalScale;

        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (collision.gameObject.CompareTag("Player") && player != null && player.isInvincible == false)
        {
            player.TakeDamage(1);
            Debug.Log("체력감소 현재체력:" + Player.health);
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
