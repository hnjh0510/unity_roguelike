using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.UIElements;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public static float health =10f;//hp�� �ִ� ü��
    public static float maxhealth = 10f;
    public static bool isInitialized = false; // �ʱ�ȭ ���θ� Ȯ���ϴ� ����
    public bool isInvincible = false; // ���� ���� ���� Ȯ��

    public Transform weaponHolder; // ������ ���� ��ġ
    private Animator anim;
    private Rigidbody2D rb;
    private Vector2 movement;
    private GameObject currentWeapon;
    public float weaponRadius = 1f; // ���Ⱑ ������ ������

    // ����Ʈ ���� �߰�
    public GameObject swordEffect;
    public GameObject bowEffect;
    public GameObject staffEffect;
    private GameObject currentEffect;

    public GameObject arrowPrefab;
    public GameObject magicPrefab;

    // ���� ���� ����
    public float attackCooldown = 1f; // ���� ��ٿ� �ð�
    private bool isAttacking = false;

    public Slider HpBar;//hp��

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // ������ Player ��ü�� �̹� �ִ� ��� ���� ������ ��ü�� �ı�
        Player[] players = FindObjectsOfType<Player>();
        if (players.Length > 1)
        {
            Destroy(gameObject);
        }

        if (!isInitialized)
        {
            isInitialized = true;
            // ���⼭ health �ʱ�ȭ ����
            health = 10f; // ���� ���� �� �ʱ� ü��
        }
    }
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void Update()
    {
        // �Է� ����
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // �ִϸ��̼� bool �Ķ���� ����
        anim.SetBool("move", movement != Vector2.zero);

        // ����Ʈ ȸ�� �� ��ġ ������Ʈ
        if (currentEffect != null)
        {
            UpdateEffectPositionAndRotation();
        }

        // ���� ó��
        if (Input.GetMouseButton(0) && !isAttacking)
        {
            StartCoroutine(Attack());
        }
        CheckHp();//ü�¹�
    }

    void FixedUpdate()
    {
        // �̵� �Է� �ޱ�
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        // �̵� ���� ����
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        // �̵� ���� ����ȭ
        movement.Normalize();  // �밢�� �̵� �� �ӵ��� �������� �ʵ��� ����ȭ

        // �÷��̾� �̵�
        rb.velocity = movement * moveSpeed;

        // �÷��̾��� ��������Ʈ ���� ����
        if (moveHorizontal != 0)
        {
            // localScale.x�� moveHorizontal�� ��ȣ�� ���� �����Ͽ� �����̳� �������� �ٶ󺸰� ��
            transform.localScale = new Vector3(Mathf.Sign(moveHorizontal)*(-4), 4, 4);
        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(BecomeInvincible());
        }
    }
    IEnumerator BecomeInvincible()
    {
        isInvincible = true;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;

            // ���� ���� ���� ���� �����̱�
            for (float i = 0; i < 0.5f; i += 0.1f)
            {
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
                yield return new WaitForSeconds(0.05f);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
                yield return new WaitForSeconds(0.05f);
            }

            // ��������Ʈ ������ ������� ����
            spriteRenderer.color = originalColor;
        }

        isInvincible = false;
    }
    void Die()
    {
        // ���� �׾��� ���� ����
        SceneManager.LoadScene(11);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // ������� �浹 ����
        if (collision.gameObject.CompareTag("Weapon"))
        {
            PickUpWeapon(collision.gameObject);
        }
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���� �ε�� �� ���� ���⸦ ������ ��� ������ ����
        RemoveUnusedItems();
    }

    void RemoveUnusedItems()
    {
        Item[] items = FindObjectsOfType<Item>();
        foreach (Item item in items)
        {
            if (item.gameObject != currentWeapon)
            {
                Destroy(item.gameObject);
            }
        }
    }
    void PickUpWeapon(GameObject newWeapon)
    {
        if (currentWeapon != null)
        {
            DropWeapon(currentWeapon);
        }

        currentWeapon = newWeapon;
        currentWeapon.transform.SetParent(weaponHolder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        // ������ SpriteRenderer ��Ȱ��ȭ
        SpriteRenderer weaponSprite = currentWeapon.GetComponent<SpriteRenderer>();
        if (weaponSprite != null)
        {
            weaponSprite.enabled = false;
        }

        // ������ Rigidbody2D�� Collider2D ����
        Rigidbody2D weaponRb = currentWeapon.GetComponent<Rigidbody2D>();
        if (weaponRb != null)
        {
            weaponRb.isKinematic = true; // ������ ��ȣ�ۿ� ��Ȱ��ȭ
            weaponRb.velocity = Vector2.zero; // �ӵ� �ʱ�ȭ
            weaponRb.angularVelocity = 0f; // ȸ�� �ӵ� �ʱ�ȭ
        }
        currentWeapon.GetComponent<Collider2D>().enabled = false; // �浹 ����

        // ���� ��ġ�� �÷��̾� ��ġ�� �ʱ�ȭ
        currentWeapon.transform.position = weaponHolder.position;

        // ���� ������ ���� ����Ʈ Ȱ��ȭ
        Weapon weaponScript = currentWeapon.GetComponent<Weapon>();
        if (weaponScript != null)
        {
            ActivateEffect(weaponScript.weaponType);
        }

        // ���� floating ��Ȱ��ȭ
        Item floatingItem = currentWeapon.GetComponent<Item>();
        if (floatingItem != null)
        {
            floatingItem.enabled = false; // ������ ��ũ��Ʈ ��Ȱ��ȭ
        }
    }

    void DropWeapon(GameObject weapon)
    {
        weapon.transform.SetParent(null);
        weapon.transform.position = transform.position; // �÷��̾� ��ġ�� ����

        // ������ SpriteRenderer Ȱ��ȭ
        SpriteRenderer weaponSprite = weapon.GetComponent<SpriteRenderer>();
        if (weaponSprite != null)
        {
            weaponSprite.enabled = true;
        }

        Collider2D weaponCollider = weapon.GetComponent<Collider2D>();
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false; // ��� �浹 ��Ȱ��ȭ
        }
        Rigidbody2D weaponRb = weapon.GetComponent<Rigidbody2D>();
        if (weaponRb != null)
        {
            weaponRb.isKinematic = false; // ������ ��ȣ�ۿ� Ȱ��ȭ
        }
        StartCoroutine(EnableColliderAfterDelay(weaponCollider, 2f));

        // ���� floating Ȱ��ȭ
        Item floatingItem = weapon.GetComponent<Item>();
        if (floatingItem != null)
        {
            floatingItem.enabled = true;
        }
    }

    System.Collections.IEnumerator EnableColliderAfterDelay(Collider2D collider, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (collider != null)
        {
            collider.enabled = true;
        }
    }

    void UpdateEffectPositionAndRotation()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Z�� �� ����

        // ����Ʈ ��ġ ������Ʈ
        Vector3 direction = (mousePosition - weaponHolder.position).normalized;
        Vector3 effectPosition = weaponHolder.position + direction * weaponRadius;
        effectPosition.z = weaponHolder.position.z; // �÷��̾� z�� ����
        currentEffect.transform.position = effectPosition;

        // ����Ʈ ȸ�� ������Ʈ
        currentEffect.transform.up = direction;
    }

    void ActivateEffect(Weapon.WeaponType weaponType)
    {
        // ��� ����Ʈ ��Ȱ��ȭ
        swordEffect.SetActive(false);
        bowEffect.SetActive(false);
        staffEffect.SetActive(false);

        // ���� ������ ���� ����Ʈ Ȱ��ȭ �� currentEffect ����
        switch (weaponType)
        {
            case Weapon.WeaponType.Sword1:
                currentEffect = swordEffect;
                break;
            case Weapon.WeaponType.Sword2:
                currentEffect = swordEffect;
                break;
            case Weapon.WeaponType.Sword3:
                currentEffect = swordEffect;
                break;
            case Weapon.WeaponType.Bow1:
                currentEffect = bowEffect;
                break;
            case Weapon.WeaponType.Bow2:
                currentEffect = bowEffect;
                break;
            case Weapon.WeaponType.Bow3:
                currentEffect = bowEffect;
                break;
            case Weapon.WeaponType.Staff1:
                currentEffect = staffEffect;
                break;
            case Weapon.WeaponType.Staff2:
                currentEffect = staffEffect;
                break;
            case Weapon.WeaponType.Staff3:
                currentEffect = staffEffect;
                break;
        }

        if (currentEffect != null)
        {
            currentEffect.SetActive(false); // ���� ��Ȱ��ȭ
        }
    }

    IEnumerator Attack()
    {
        if (isAttacking) yield break;
        isAttacking = true;
        if (currentEffect != null)
        {
            currentEffect.SetActive(true);
            // ���� ��� �ݶ��̴��� ��� Ȱ��ȭ
            if (currentWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Sword1)
            {
                Collider2D effectCollider = currentEffect.GetComponent<Collider2D>();
                if (effectCollider != null)
                {
                    effectCollider.enabled = true;
                    yield return new WaitForSeconds(0.3f);  // �ݶ��̴��� Ȱ��ȭ�Ǿ� �ִ� �ð�
                    effectCollider.enabled = false;
                }
                // ����Ʈ ��Ȱ��ȭ ���� �ݶ��̴��� ��Ȱ��ȭ Ȯ���� �ϱ�
                if (currentEffect.GetComponent<Collider2D>() != null)
                {
                    currentEffect.GetComponent<Collider2D>().enabled = false;
                }
                currentEffect.SetActive(false); // ����Ʈ ��Ȱ��ȭ
            }
            if (currentWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Sword2)
            {
                Collider2D effectCollider = currentEffect.GetComponent<Collider2D>();
                if (effectCollider != null)
                {
                    // �ݶ��̴��� ó�� Ȱ��ȭ�ϰ� 0.15�� �� ��Ȱ��ȭ
                    effectCollider.enabled = true;
                    yield return new WaitForSeconds(0.2f);  // �ݶ��̴��� Ȱ��ȭ�Ǿ� �ִ� �ð�
                    effectCollider.enabled = false;
                    currentEffect.SetActive(false);

                    currentEffect.SetActive(true);
                    // ª�� ��� �� �ݶ��̴��� �ٽ� Ȱ��ȭ�ϰ� 0.15�� �� ��Ȱ��ȭ
                    yield return new WaitForSeconds(0.05f);  // �� ��° Ȱ��ȭ �� ��� ���
                    effectCollider.enabled = true;
                    yield return new WaitForSeconds(0.2f);  // �� ��° �ݶ��̴� Ȱ��ȭ �ð�
                    effectCollider.enabled = false;
                }

                // ����Ʈ ��Ȱ��ȭ ���� �ݶ��̴��� ��Ȱ��ȭ Ȯ���� �ϱ�
                if (currentEffect.GetComponent<Collider2D>() != null)
                {
                    currentEffect.GetComponent<Collider2D>().enabled = false;
                }
                currentEffect.SetActive(false); // ����Ʈ ��Ȱ��ȭ
            }
            if (currentWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Sword3)
            {
                Collider2D effectCollider = currentEffect.GetComponent<Collider2D>();
                if (effectCollider != null)
                {
                    // �ݶ��̴��� ó�� Ȱ��ȭ�ϰ� 0.15�� �� ��Ȱ��ȭ
                    effectCollider.enabled = true;
                    yield return new WaitForSeconds(0.15f);  // �ݶ��̴��� Ȱ��ȭ�Ǿ� �ִ� �ð�
                    effectCollider.enabled = false;

                    currentEffect.SetActive(false);
                    currentEffect.SetActive(true);
                    // ª�� ��� �� �ݶ��̴��� �ٽ� Ȱ��ȭ�ϰ� 0.15�� �� ��Ȱ��ȭ
                    yield return new WaitForSeconds(0.05f);  // �� ��° Ȱ��ȭ �� ��� ���
                    effectCollider.enabled = true;
                    yield return new WaitForSeconds(0.15f);  // �� ��° �ݶ��̴� Ȱ��ȭ �ð�
                    effectCollider.enabled = false;

                    currentEffect.SetActive(false);
                    currentEffect.SetActive(true);
                    // ª�� ��� �� �ݶ��̴��� �ٽ� Ȱ��ȭ�ϰ� 0.15�� �� ��Ȱ��ȭ
                    yield return new WaitForSeconds(0.05f);  // �� ��° Ȱ��ȭ �� ��� ���
                    effectCollider.enabled = true;
                    yield return new WaitForSeconds(0.15f);  // �� ��° �ݶ��̴� Ȱ��ȭ �ð�
                    effectCollider.enabled = false;
                }

                // ����Ʈ ��Ȱ��ȭ ���� �ݶ��̴��� ��Ȱ��ȭ Ȯ���� �ϱ�
                if (currentEffect.GetComponent<Collider2D>() != null)
                {
                    currentEffect.GetComponent<Collider2D>().enabled = false;
                }
                currentEffect.SetActive(false); // ����Ʈ ��Ȱ��ȭ
            }

            // Ȱ�� ��� ȭ�� �߻�
            else if (currentWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Bow1)
            {
                LaunchProjectile(arrowPrefab,0, 5);
            }
            else if (currentWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Bow2)
            {
                // ������ ���ݾ� �����Ͽ� ȭ�� �� ���� �߻�
                LaunchProjectile(arrowPrefab, -10, 5);  // ù ��° ȭ���� ���� ����
                LaunchProjectile(arrowPrefab, 10, 5);   // �� ��° ȭ���� ���� ����
            }
            // �������� ��� ���� ������Ÿ�� �߻�
            else if (currentWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Bow3)
            {
                // ������ ���ݾ� �����Ͽ� ȭ�� �� ���� �߻�
                LaunchProjectile(arrowPrefab, -15, 5);  // ù ��° ȭ���� ���� ����
                LaunchProjectile(arrowPrefab, 0, 5);   // �� ��° ȭ���� ���
                LaunchProjectile(arrowPrefab, 15, 5);   // �� ��° ȭ���� ���� ����
            }
            else if (currentWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Staff1)
            {
                LaunchProjectile(magicPrefab, 0, 5);
            }
            else if (currentWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Staff2)
            {
                // ������ ���ݾ� �����Ͽ� ȭ�� �� ���� �߻�
                LaunchProjectile(magicPrefab, -10, 5);  // ù ��° ȭ���� ���� ����
                LaunchProjectile(magicPrefab, 10, 5);   // �� ��° ȭ���� ���� ����
            }
            else if (currentWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Staff3)
            {
                // ������ ���ݾ� �����Ͽ� ȭ�� �� ���� �߻�
                LaunchProjectile(magicPrefab, -15, 5);  // ù ��° ȭ���� ���� ����
                LaunchProjectile(magicPrefab, 0, 5);   // �� ��° ȭ���� ���
                LaunchProjectile(magicPrefab, 15, 5);   // �� ��° ȭ���� ���� ����
            }

            yield return new WaitForSeconds(0.3f);  // ����Ʈ ���� �ð�
            currentEffect.SetActive(false);
        }
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;

        if (Input.GetMouseButton(0))
        {
            StartCoroutine(Attack());
        }
    }

    void LaunchProjectile(GameObject projectilePrefab, float angle, float speed)
    {
        if (projectilePrefab != null)
        {
            Vector3 firingPosition = transform.position; // �߻� ��ġ ����
            GameObject projectile = Instantiate(projectilePrefab, firingPosition, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

            // ���콺 ������ ��ġ �������� �߻� ���� ���
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0; // z-�� ���� 0���� ����

            // �߻� ���� ���� ���� �� ���� ����
            Vector3 firingDirection = (targetPosition - firingPosition).normalized;
            firingDirection = Quaternion.Euler(0, 0, angle) * firingDirection;

            // Rigidbody2D�� velocity�� �߻� ����� �ӵ� ����
            rb.velocity = firingDirection * speed;

            // ȭ���� ȸ�� ������ �߻� ���⿡ ���߾� ����
            float rotZ = Mathf.Atan2(firingDirection.y, firingDirection.x) * Mathf.Rad2Deg;

            // ȭ�� ��������Ʈ�� �⺻������ 45�� ȸ���� ���·� ���۵Ǿ��ٸ�, �̸� ����
            projectile.transform.rotation = Quaternion.Euler(0, 0, rotZ - 135);
        }
    }

    public void CheckHp()
    {
        if(HpBar != null)
        {
            HpBar.value = health/maxhealth;
        }
    }
}