using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public string enemyname;  // 보스의 이름
    public float speed;  // 이동 속도
    public float health;  // 체력

    public GameObject bullet;  // 발사할 총알의 프리팹
    public GameObject player;  // 플레이어 오브젝트

    public float maxShotDelay; // 최대 샷 딜레이
    public float curShotDelay; // 현재 샷 딜레이

    Animator anim;
    Rigidbody2D rigid;

    public bool toMove;
    public GameObject isInside;

    public GameObject potal;

    private Dictionary<string, List<int>> enemyPatterns = new Dictionary<string, List<int>>();  // 각 보스 유형별 패턴 저장
    public int currentPatternIndex = 0;  // 현재 패턴 인덱스
    private List<int> currentPatterns;  // 현재 보스의 패턴 목록

    private Vector3 randomDirection;  // 무작위 이동 방향
    private float directionChangeInterval = 2.0f;  // 방향 변경 간격
    private float directionChangeTimer;  // 방향 변경 타이머

    private float patternChangeInterval = 5f;  // 패턴 변경 간격
    private float patternChangeTimer;  // 패턴 변경 타이머

    // RushAndBoom 관련 변수
    private bool isBooming = false;  // 현재 Boom 상태인지 여부
    private float boomDuration = 1.0f;  // Boom 지속 시간
    private float boomTimer = 0f;  // Boom 타이머
    private Vector3 originalScale;  // 원래 크기

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        randomDirection = GetRandomDirection();
        directionChangeTimer = directionChangeInterval;
        patternChangeTimer = patternChangeInterval;

        // 각 적 유형별 패턴 정의
        enemyPatterns.Add("A", new List<int> { 0, 1, 6 });
        enemyPatterns.Add("B", new List<int> { 1, 3, 6, 5 });
        enemyPatterns.Add("C", new List<int> { 2, 6, 4, 7, 2, 6, 5, 2, 4, 6 });

        // 보스의 이름에 따라 해당하는 패턴 설정
        if (enemyPatterns.ContainsKey(enemyname))
        {
            currentPatterns = enemyPatterns[enemyname];
        }
        else
        {
            currentPatterns = new List<int> { 0 }; // 지정되지 않은 경우 기본 패턴 사용
        }

        // 원래 크기 저장
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (health <= 0)
        {
            rigid.velocity = Vector2.zero;
            Destroy(gameObject);  // 체력이 0 이하이면 보스 제거
            return;
        }

        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0)
        {
            randomDirection = GetRandomDirection();  // 새로운 방향 설정
            directionChangeTimer = directionChangeInterval;
        }

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
            patternChangeTimer -= Time.deltaTime;
            if (patternChangeTimer <= 0)
            {
                currentPatternIndex = (currentPatternIndex + 1) % currentPatterns.Count;  // 패턴 인덱스 업데이트
                patternChangeTimer = patternChangeInterval;  // 패턴 변경 타이머 리셋
            }

            ExecutePattern(currentPatterns[currentPatternIndex]);  // 현재 패턴 실행
            Reload();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        // 플레이어와의 충돌 시 물리적 반응을 무시하고, 체력만 감소시킴
        if (collision.gameObject.CompareTag("Player") && player != null && !player.isInvincible)
        {
            player.TakeDamage(1);  // 플레이어의 체력 감소
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // 플레이어와의 지속적인 충돌을 무시
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }

    public void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce)
    {
        health -= damage;

        // 보스를 약간 밀어냄
        rigid.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // 적이 죽었을 때의 로직
        Destroy(gameObject);

        Instantiate(potal, transform.position, Quaternion.identity);
    }

    Vector3 GetRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);  // 랜덤 각도로 방향 결정
    }

    void ExecutePattern(int patternIndex)
    {
        if (curShotDelay >= maxShotDelay)
        {
            Vector3 dirVec = player.transform.position - transform.position;
            Vector2 dirVec2D = new Vector2(dirVec.x, dirVec.y).normalized;
            switch (patternIndex)
            {
                case 0:
                    MoveRandomly();
                    FireDirect(dirVec, 1f);
                    break;
                case 1:
                    MoveRandomly();
                    FireDirect2(dirVec, 1f);
                    break;
                case 2:
                    MoveRandomly();
                    FireDirect3(dirVec, 1f);
                    break;
                case 3:
                    MoveRandomly();
                    FirelittleCircular();
                    break;
                case 4:
                    MoveRandomly();
                    FireWobblingBullets();
                    break;
                case 5:
                    FireCircular();
                    break;
                case 6:
                    Rush();
                    break;
                case 7:
                    RushAndBoom();
                    break;
            }
            curShotDelay = 0;
        }
    }

    void MoveRandomly()
    {
        if (anim != null)
        {
            anim.SetBool("B_move", true);
        }
        rigid.velocity = randomDirection * speed;
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

    void FirelittleCircular()
    {
        int numberOfDirections = 4;
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
    void FireWobblingBullets()
    {
        int numberOfDirections = 4;
        float angleStep = 360f / numberOfDirections;
        float wobbleFrequency = 10f;  // 주기 설정
        float wobbleAmplitude = 40f;  // 진폭 설정

        for (int i = 0; i < numberOfDirections; i++)
        {
            float angle = i * angleStep;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, angle - 90));
            Rigidbody2D bulletRigidbody = newBullet.GetComponent<Rigidbody2D>();
            StartCoroutine(WobbleBullet(bulletRigidbody, direction, wobbleFrequency, wobbleAmplitude));
        }
    }

    IEnumerator WobbleBullet(Rigidbody2D bulletRigidbody, Vector2 initialDirection, float frequency, float amplitude)
    {
        float time = 0f;

        while (bulletRigidbody != null)
        {
            float angleOffset = Mathf.Sin(time * frequency) * amplitude;
            Vector2 newDirection = Quaternion.Euler(0, 0, angleOffset) * initialDirection;
            bulletRigidbody.velocity = newDirection * 5;

            time += Time.deltaTime;
            yield return null;
        }
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

    void Rush()
    {
        Vector3 dirVec = player.transform.position - transform.position;
        Vector2 dirVec2D = new Vector2(dirVec.x, dirVec.y).normalized;
        rigid.velocity = dirVec2D * speed * 3f;
        if (anim != null)
        {
            anim.SetBool("B_move", true);
        }
    }

    void RushAndBoom()
    {
        if (!isBooming)
        {
            // Rush 실행
            StartCoroutine(RushAndBoomCoroutine());
        }
    }

    IEnumerator RushAndBoomCoroutine()
    {
        isBooming = true;

        // Rush
        Vector3 dirVec = player.transform.position - transform.position;
        Vector2 dirVec2D = new Vector2(dirVec.x, dirVec.y).normalized;
        rigid.velocity = dirVec2D * speed * 2f;

        // 크기 증가
        float elapsedTime = 0f;
        while (elapsedTime < boomDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.5f, elapsedTime / boomDuration);
            elapsedTime += Time.deltaTime;

            // 계속해서 플레이어를 향해 이동
            dirVec = player.transform.position - transform.position;
            dirVec2D = new Vector2(dirVec.x, dirVec.y).normalized;
            rigid.velocity = dirVec2D * speed * 1.7f;

            yield return null;
        }

        // 크기 감소
        elapsedTime = 0f;
        while (elapsedTime < boomDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale * 1.5f, originalScale, elapsedTime / boomDuration);
            elapsedTime += Time.deltaTime;

            // 계속해서 플레이어를 향해 이동
            dirVec = player.transform.position - transform.position;
            dirVec2D = new Vector2(dirVec.x, dirVec.y).normalized;
            rigid.velocity = dirVec2D * speed * 1.7f;

            yield return null;
        }

        isBooming = false;
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
