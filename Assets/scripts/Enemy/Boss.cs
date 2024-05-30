using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public string enemyname;  // ������ �̸�
    public float speed;  // �̵� �ӵ�
    public float health;  // ü��

    public GameObject bullet;  // �߻��� �Ѿ��� ������
    public GameObject player;  // �÷��̾� ������Ʈ

    Animator anim;
    Rigidbody2D rigid;

    private Dictionary<string, List<int>> enemyPatterns = new Dictionary<string, List<int>>();  // �� ���� ������ ���� ����
    public int currentPatternIndex = 0;  // ���� ���� �ε���
    private List<int> currentPatterns;  // ���� ������ ���� ���

    private Vector3 randomDirection;  // ������ �̵� ����
    private float directionChangeInterval = 2.0f;  // ���� ���� ����
    private float directionChangeTimer;  // ���� ���� Ÿ�̸�

    private float patternChangeInterval = 10.0f;  // ���� ���� ����
    private float patternChangeTimer;  // ���� ���� Ÿ�̸�

    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        randomDirection = GetRandomDirection();
        directionChangeTimer = directionChangeInterval;
        patternChangeTimer = patternChangeInterval;

        // �� �� ������ ���� ����
        enemyPatterns.Add("A", new List<int> { 0, 1 });
        enemyPatterns.Add("B", new List<int> { 0, 1, 2 });

        // ������ �̸��� ���� �ش��ϴ� ���� ����
        if (enemyPatterns.ContainsKey(enemyname))
        {
            currentPatterns = enemyPatterns[enemyname];
        }
        else
        {
            currentPatterns = new List<int> { 0 }; // �������� ���� ��� �⺻ ���� ���
        }
    }

    void Update()
    {
        if (health <= 0)
        {
            rigid.velocity = Vector2.zero;
            Destroy(gameObject);  // ü���� 0 �����̸� ���� ����
            return;
        }

        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0)
        {
            randomDirection = GetRandomDirection();  // ���ο� ���� ����
            directionChangeTimer = directionChangeInterval;
        }

        patternChangeTimer -= Time.deltaTime;
        if (patternChangeTimer <= 0)
        {
            currentPatternIndex = (currentPatternIndex + 1) % currentPatterns.Count;  // ���� �ε��� ������Ʈ
            patternChangeTimer = patternChangeInterval;  // ���� ���� Ÿ�̸� ����
        }

        ExecutePattern(currentPatterns[currentPatternIndex]);  // ���� ���� ����
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

    Vector3 GetRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);  // ���� ������ ���� ����
    }

    void ExecutePattern(int patternIndex)
    {
        switch (patternIndex)
        {
            case 0:
                CancelInvoke("FireCircular");
                MoveRandomly();  // ������ �̵�
                if (!IsInvoking("Fire3"))
                {
                    InvokeRepeating("Fire3", 1.5f, 1.5f);  // �Ѿ� �߻� �簳
                }
                break;
            case 1:
                Rush();  // ����
                CancelInvoke("Fire3");
                CancelInvoke("FireCircular");
                break;
            case 2:
                CancelInvoke("Fire3");
                if (!IsInvoking("FireCircular"))
                {
                    InvokeRepeating("FireCircular", 3f, 3f);  // �Ѿ� �߻� �簳
                }
                break;
        }
    }

    void MoveRandomly()
    {
        rigid.velocity = randomDirection * speed;  // ������ �������� �̵�
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
        rigid.velocity = dirVec2D * speed * 2;  // �÷��̾� �������� 2�� ������ ����
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