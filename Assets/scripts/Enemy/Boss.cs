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

    Animator anim;
    Rigidbody2D rigid;

    private Dictionary<string, List<int>> enemyPatterns = new Dictionary<string, List<int>>();  // 각 보스 유형별 패턴 저장
    public int currentPatternIndex = 0;  // 현재 패턴 인덱스
    private List<int> currentPatterns;  // 현재 보스의 패턴 목록

    private Vector3 randomDirection;  // 무작위 이동 방향
    private float directionChangeInterval = 2.0f;  // 방향 변경 간격
    private float directionChangeTimer;  // 방향 변경 타이머

    private float patternChangeInterval = 10.0f;  // 패턴 변경 간격
    private float patternChangeTimer;  // 패턴 변경 타이머

    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        randomDirection = GetRandomDirection();
        directionChangeTimer = directionChangeInterval;
        patternChangeTimer = patternChangeInterval;

        // 각 적 유형별 패턴 정의
        enemyPatterns.Add("A", new List<int> { 0, 1 });
        enemyPatterns.Add("B", new List<int> { 0, 1, 2 });

        // 보스의 이름에 따라 해당하는 패턴 설정
        if (enemyPatterns.ContainsKey(enemyname))
        {
            currentPatterns = enemyPatterns[enemyname];
        }
        else
        {
            currentPatterns = new List<int> { 0 }; // 지정되지 않은 경우 기본 패턴 사용
        }
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

        patternChangeTimer -= Time.deltaTime;
        if (patternChangeTimer <= 0)
        {
            currentPatternIndex = (currentPatternIndex + 1) % currentPatterns.Count;  // 패턴 인덱스 업데이트
            patternChangeTimer = patternChangeInterval;  // 패턴 변경 타이머 리셋
        }

        ExecutePattern(currentPatterns[currentPatternIndex]);  // 현재 패턴 실행
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
        // 적이 죽었을 때의 로직
        Destroy(gameObject);
    }

    Vector3 GetRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);  // 랜덤 각도로 방향 결정
    }

    void ExecutePattern(int patternIndex)
    {
        switch (patternIndex)
        {
            case 0:
                CancelInvoke("FireCircular");
                MoveRandomly();  // 무작위 이동
                if (!IsInvoking("Fire3"))
                {
                    InvokeRepeating("Fire3", 1.5f, 1.5f);  // 총알 발사 재개
                }
                break;
            case 1:
                Rush();  // 돌진
                CancelInvoke("Fire3");
                CancelInvoke("FireCircular");
                break;
            case 2:
                CancelInvoke("Fire3");
                if (!IsInvoking("FireCircular"))
                {
                    InvokeRepeating("FireCircular", 3f, 3f);  // 총알 발사 재개
                }
                break;
        }
    }

    void MoveRandomly()
    {
        rigid.velocity = randomDirection * speed;  // 설정된 방향으로 이동
        anim.SetBool("B_move", true);
    }
    void Fire3()
    {
        int numberOfDirections = 3;
        float angleStep = 360f / numberOfDirections;
        for (int i = 0; i < numberOfDirections; i++)
        {
            float angle = i * angleStep;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, angle));
            Rigidbody2D bulletRigidbody = newBullet.GetComponent<Rigidbody2D>();
            bulletRigidbody.velocity = direction * 5;
        }
    }

    void Rush()
    {
        Vector3 dirVec = player.transform.position - transform.position;
        Vector2 dirVec2D = new Vector2(dirVec.x, dirVec.y).normalized;
        rigid.velocity = dirVec2D * speed * 2;  // 플레이어 방향으로 2배 빠르게 돌진
        anim.SetBool("B_move", true);
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
}