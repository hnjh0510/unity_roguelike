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

    public float maxShotDelay; // �ִ� �� ������
    public float curShotDelay; // ���� �� ������

    Animator anim;
    Rigidbody2D rigid;

    public bool toMove;
    public GameObject isInside;

    public GameObject potal;

    private Dictionary<string, List<int>> enemyPatterns = new Dictionary<string, List<int>>();  // �� ���� ������ ���� ����
    public int currentPatternIndex = 0;  // ���� ���� �ε���
    private List<int> currentPatterns;  // ���� ������ ���� ���

    private Vector3 randomDirection;  // ������ �̵� ����
    private float directionChangeInterval = 2.0f;  // ���� ���� ����
    private float directionChangeTimer;  // ���� ���� Ÿ�̸�

    private float patternChangeInterval = 5f;  // ���� ���� ����
    private float patternChangeTimer;  // ���� ���� Ÿ�̸�

    // RushAndBoom ���� ����
    private bool isBooming = false;  // ���� Boom �������� ����
    private float boomDuration = 1.0f;  // Boom ���� �ð�
    private float boomTimer = 0f;  // Boom Ÿ�̸�
    private Vector3 originalScale;  // ���� ũ��

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

        // �� �� ������ ���� ����
        enemyPatterns.Add("A", new List<int> { 0, 1, 6 });
        enemyPatterns.Add("B", new List<int> { 1, 3, 6, 5 });
        enemyPatterns.Add("C", new List<int> { 2, 6, 4, 7, 2, 6, 5, 2, 4, 6 });

        // ������ �̸��� ���� �ش��ϴ� ���� ����
        if (enemyPatterns.ContainsKey(enemyname))
        {
            currentPatterns = enemyPatterns[enemyname];
        }
        else
        {
            currentPatterns = new List<int> { 0 }; // �������� ���� ��� �⺻ ���� ���
        }

        // ���� ũ�� ����
        originalScale = transform.localScale;
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
                currentPatternIndex = (currentPatternIndex + 1) % currentPatterns.Count;  // ���� �ε��� ������Ʈ
                patternChangeTimer = patternChangeInterval;  // ���� ���� Ÿ�̸� ����
            }

            ExecutePattern(currentPatterns[currentPatternIndex]);  // ���� ���� ����
            Reload();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        // �÷��̾���� �浹 �� ������ ������ �����ϰ�, ü�¸� ���ҽ�Ŵ
        if (collision.gameObject.CompareTag("Player") && player != null && !player.isInvincible)
        {
            player.TakeDamage(1);  // �÷��̾��� ü�� ����
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // �÷��̾���� �������� �浹�� ����
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }

    public void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce)
    {
        health -= damage;

        // ������ �ణ �о
        rigid.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // ���� �׾��� ���� ����
        Destroy(gameObject);

        Instantiate(potal, transform.position, Quaternion.identity);
    }

    Vector3 GetRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);  // ���� ������ ���� ����
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
        float wobbleFrequency = 10f;  // �ֱ� ����
        float wobbleAmplitude = 40f;  // ���� ����

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
            // Rush ����
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

        // ũ�� ����
        float elapsedTime = 0f;
        while (elapsedTime < boomDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.5f, elapsedTime / boomDuration);
            elapsedTime += Time.deltaTime;

            // ����ؼ� �÷��̾ ���� �̵�
            dirVec = player.transform.position - transform.position;
            dirVec2D = new Vector2(dirVec.x, dirVec.y).normalized;
            rigid.velocity = dirVec2D * speed * 1.7f;

            yield return null;
        }

        // ũ�� ����
        elapsedTime = 0f;
        while (elapsedTime < boomDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale * 1.5f, originalScale, elapsedTime / boomDuration);
            elapsedTime += Time.deltaTime;

            // ����ؼ� �÷��̾ ���� �̵�
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
