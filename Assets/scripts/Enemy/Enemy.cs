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
    public bool isBig = false; // 공격 중인지 여부 체크

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
        // 아이템 드랍 확률 (예: 50% 확률로 아이템 드랍)
        float dropChance = 0.5f;
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
                        // 타입 A의 공격 로직
                        FireDirect(dirVec2D, 2);
                        break;
                    case "B":
                        // 타입 B는 총알을 발사하지 않고 속도 증가
                        rigid.velocity = dirVec2D * speed * 1.5f;
                        break;
                    case "C":
                        // 타입 C의 공격 로직
                        FireCircular();
                        break;
                    case "D":
                        FireDirect2(dirVec2D, 2);
                        break;
                    case "E":
                        rigid.velocity = dirVec2D * speed * 1.5f;
                        Big();                    
                        break;
                    case "F":
                        break;
                    case "G":
                        break;
                    case "H":
                        break;
                    case "I":
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
        // 총알 발사 위치 오프셋 설정
        Vector3 rightOffset = new Vector3(0.2f, 0, 0); // 오른쪽으로 약간 이동한 위치
        Vector3 leftOffset = new Vector3(-0.2f, 0, 0); // 왼쪽으로 약간 이동한 위치

        // 오른쪽 총알 발사
        GameObject rightBullet = Instantiate(bullet, transform.position + rightOffset, Quaternion.identity);
        Rigidbody2D rightBulletRigidbody = rightBullet.GetComponent<Rigidbody2D>();
        rightBulletRigidbody.AddForce(direction * force, ForceMode2D.Impulse);

        // 왼쪽 총알 발사
        GameObject leftBullet = Instantiate(bullet, transform.position + leftOffset, Quaternion.identity);
        Rigidbody2D leftBulletRigidbody = leftBullet.GetComponent<Rigidbody2D>();
        leftBulletRigidbody.AddForce(direction * force, ForceMode2D.Impulse);
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
            bulletRigidbody.velocity = direction * 2f;
        }
    }
    void Big()
    {
        if (!isBig) // 이미 커진 상태가 아니라면
        {
            transform.localScale = new Vector3(0.25f, 0.25f, 0.25f); // 다른걸로 고쳐야됨
            isBig = true; // 커진 상태로 설정
        }
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
