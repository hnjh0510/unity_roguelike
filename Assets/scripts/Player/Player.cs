using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.UIElements;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public static float health =10f;//hp바 최대 체력
    public static float maxhealth = 10f;
    public static bool isInitialized = false; // 초기화 여부를 확인하는 변수
    public bool isInvincible = false; // 무적 상태 여부 확인

    public Transform weaponHolder; // 공격이 나갈 위치
    private Animator anim;
    private Rigidbody2D rb;
    private Vector2 movement;
    private GameObject currentWeapon;
    public float weaponRadius = 1f; // 무기가 움직일 반지름

    // 이펙트 변수 추가
    public GameObject swordEffect;
    public GameObject bowEffect;
    public GameObject staffEffect;
    private GameObject currentEffect;

    public GameObject arrowPrefab;
    public GameObject magicPrefab;

    // 공격 관련 변수
    public float attackCooldown = 1f; // 공격 쿨다운 시간
    private bool isAttacking = false;

    public Slider HpBar;//hp바

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // 동일한 Player 객체가 이미 있는 경우 새로 생성된 객체를 파괴
        Player[] players = FindObjectsOfType<Player>();
        if (players.Length > 1)
        {
            Destroy(gameObject);
        }

        if (!isInitialized)
        {
            isInitialized = true;
            // 여기서 health 초기화 가능
            health = 10f; // 게임 시작 시 초기 체력
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
        // 입력 감지
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // 애니메이션 bool 파라미터 설정
        anim.SetBool("move", movement != Vector2.zero);

        // 이펙트 회전 및 위치 업데이트
        if (currentEffect != null)
        {
            UpdateEffectPositionAndRotation();
        }

        // 공격 처리
        if (Input.GetMouseButton(0) && !isAttacking)
        {
            StartCoroutine(Attack());
        }
        CheckHp();//체력바
    }

    void FixedUpdate()
    {
        // 이동 입력 받기
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        // 이동 벡터 생성
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        // 이동 벡터 정규화
        movement.Normalize();  // 대각선 이동 시 속도가 증가하지 않도록 정규화

        // 플레이어 이동
        rb.velocity = movement * moveSpeed;

        // 플레이어의 스프라이트 방향 조정
        if (moveHorizontal != 0)
        {
            // localScale.x를 moveHorizontal의 부호에 따라 조정하여 왼쪽이나 오른쪽을 바라보게 함
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

            // 무적 상태 동안 색깔 깜빡이기
            for (float i = 0; i < 0.5f; i += 0.1f)
            {
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
                yield return new WaitForSeconds(0.05f);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
                yield return new WaitForSeconds(0.05f);
            }

            // 스프라이트 색깔을 원래대로 복구
            spriteRenderer.color = originalColor;
        }

        isInvincible = false;
    }
    void Die()
    {
        // 적이 죽었을 때의 로직
        SceneManager.LoadScene(11);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 무기와의 충돌 감지
        if (collision.gameObject.CompareTag("Weapon"))
        {
            PickUpWeapon(collision.gameObject);
        }
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드될 때 현재 무기를 제외한 모든 아이템 제거
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

        // 무기의 SpriteRenderer 비활성화
        SpriteRenderer weaponSprite = currentWeapon.GetComponent<SpriteRenderer>();
        if (weaponSprite != null)
        {
            weaponSprite.enabled = false;
        }

        // 무기의 Rigidbody2D와 Collider2D 설정
        Rigidbody2D weaponRb = currentWeapon.GetComponent<Rigidbody2D>();
        if (weaponRb != null)
        {
            weaponRb.isKinematic = true; // 물리적 상호작용 비활성화
            weaponRb.velocity = Vector2.zero; // 속도 초기화
            weaponRb.angularVelocity = 0f; // 회전 속도 초기화
        }
        currentWeapon.GetComponent<Collider2D>().enabled = false; // 충돌 방지

        // 무기 위치를 플레이어 위치로 초기화
        currentWeapon.transform.position = weaponHolder.position;

        // 무기 종류에 따른 이펙트 활성화
        Weapon weaponScript = currentWeapon.GetComponent<Weapon>();
        if (weaponScript != null)
        {
            ActivateEffect(weaponScript.weaponType);
        }

        // 무기 floating 비활성화
        Item floatingItem = currentWeapon.GetComponent<Item>();
        if (floatingItem != null)
        {
            floatingItem.enabled = false; // 아이템 스크립트 비활성화
        }
    }

    void DropWeapon(GameObject weapon)
    {
        weapon.transform.SetParent(null);
        weapon.transform.position = transform.position; // 플레이어 위치로 설정

        // 무기의 SpriteRenderer 활성화
        SpriteRenderer weaponSprite = weapon.GetComponent<SpriteRenderer>();
        if (weaponSprite != null)
        {
            weaponSprite.enabled = true;
        }

        Collider2D weaponCollider = weapon.GetComponent<Collider2D>();
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false; // 잠시 충돌 비활성화
        }
        Rigidbody2D weaponRb = weapon.GetComponent<Rigidbody2D>();
        if (weaponRb != null)
        {
            weaponRb.isKinematic = false; // 물리적 상호작용 활성화
        }
        StartCoroutine(EnableColliderAfterDelay(weaponCollider, 2f));

        // 무기 floating 활성화
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
        mousePosition.z = 0; // Z축 값 고정

        // 이펙트 위치 업데이트
        Vector3 direction = (mousePosition - weaponHolder.position).normalized;
        Vector3 effectPosition = weaponHolder.position + direction * weaponRadius;
        effectPosition.z = weaponHolder.position.z; // 플레이어 z값 유지
        currentEffect.transform.position = effectPosition;

        // 이펙트 회전 업데이트
        currentEffect.transform.up = direction;
    }

    void ActivateEffect(Weapon.WeaponType weaponType)
    {
        // 모든 이펙트 비활성화
        swordEffect.SetActive(false);
        bowEffect.SetActive(false);
        staffEffect.SetActive(false);

        // 무기 종류에 따른 이펙트 활성화 및 currentEffect 설정
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
            currentEffect.SetActive(false); // 먼저 비활성화
        }
    }

    IEnumerator Attack()
    {
        if (isAttacking) yield break;
        isAttacking = true;
        if (currentEffect != null)
        {
            currentEffect.SetActive(true);
            // 검의 경우 콜라이더를 잠시 활성화
            if (currentWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Sword1)
            {
                Collider2D effectCollider = currentEffect.GetComponent<Collider2D>();
                if (effectCollider != null)
                {
                    effectCollider.enabled = true;
                    yield return new WaitForSeconds(0.3f);  // 콜라이더가 활성화되어 있는 시간
                    effectCollider.enabled = false;
                }
                // 이팩트 비활성화 전에 콜라이더를 비활성화 확실히 하기
                if (currentEffect.GetComponent<Collider2D>() != null)
                {
                    currentEffect.GetComponent<Collider2D>().enabled = false;
                }
                currentEffect.SetActive(false); // 이팩트 비활성화
            }
            if (currentWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Sword2)
            {
                Collider2D effectCollider = currentEffect.GetComponent<Collider2D>();
                if (effectCollider != null)
                {
                    // 콜라이더를 처음 활성화하고 0.15초 후 비활성화
                    effectCollider.enabled = true;
                    yield return new WaitForSeconds(0.2f);  // 콜라이더가 활성화되어 있는 시간
                    effectCollider.enabled = false;
                    currentEffect.SetActive(false);

                    currentEffect.SetActive(true);
                    // 짧은 대기 후 콜라이더를 다시 활성화하고 0.15초 후 비활성화
                    yield return new WaitForSeconds(0.05f);  // 두 번째 활성화 전 잠시 대기
                    effectCollider.enabled = true;
                    yield return new WaitForSeconds(0.2f);  // 두 번째 콜라이더 활성화 시간
                    effectCollider.enabled = false;
                }

                // 이팩트 비활성화 전에 콜라이더를 비활성화 확실히 하기
                if (currentEffect.GetComponent<Collider2D>() != null)
                {
                    currentEffect.GetComponent<Collider2D>().enabled = false;
                }
                currentEffect.SetActive(false); // 이팩트 비활성화
            }
            if (currentWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Sword3)
            {
                Collider2D effectCollider = currentEffect.GetComponent<Collider2D>();
                if (effectCollider != null)
                {
                    // 콜라이더를 처음 활성화하고 0.15초 후 비활성화
                    effectCollider.enabled = true;
                    yield return new WaitForSeconds(0.15f);  // 콜라이더가 활성화되어 있는 시간
                    effectCollider.enabled = false;

                    currentEffect.SetActive(false);
                    currentEffect.SetActive(true);
                    // 짧은 대기 후 콜라이더를 다시 활성화하고 0.15초 후 비활성화
                    yield return new WaitForSeconds(0.05f);  // 두 번째 활성화 전 잠시 대기
                    effectCollider.enabled = true;
                    yield return new WaitForSeconds(0.15f);  // 두 번째 콜라이더 활성화 시간
                    effectCollider.enabled = false;

                    currentEffect.SetActive(false);
                    currentEffect.SetActive(true);
                    // 짧은 대기 후 콜라이더를 다시 활성화하고 0.15초 후 비활성화
                    yield return new WaitForSeconds(0.05f);  // 두 번째 활성화 전 잠시 대기
                    effectCollider.enabled = true;
                    yield return new WaitForSeconds(0.15f);  // 두 번째 콜라이더 활성화 시간
                    effectCollider.enabled = false;
                }

                // 이팩트 비활성화 전에 콜라이더를 비활성화 확실히 하기
                if (currentEffect.GetComponent<Collider2D>() != null)
                {
                    currentEffect.GetComponent<Collider2D>().enabled = false;
                }
                currentEffect.SetActive(false); // 이팩트 비활성화
            }

            // 활의 경우 화살 발사
            else if (currentWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Bow1)
            {
                LaunchProjectile(arrowPrefab,0, 5);
            }
            else if (currentWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Bow2)
            {
                // 각도를 조금씩 변경하여 화살 두 개를 발사
                LaunchProjectile(arrowPrefab, -10, 5);  // 첫 번째 화살은 좌측 각도
                LaunchProjectile(arrowPrefab, 10, 5);   // 두 번째 화살은 우측 각도
            }
            // 스태프의 경우 마법 프로젝타일 발사
            else if (currentWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Bow3)
            {
                // 각도를 조금씩 변경하여 화살 세 개를 발사
                LaunchProjectile(arrowPrefab, -15, 5);  // 첫 번째 화살은 좌측 각도
                LaunchProjectile(arrowPrefab, 0, 5);   // 두 번째 화살은 가운데
                LaunchProjectile(arrowPrefab, 15, 5);   // 세 번째 화살은 우측 각도
            }
            else if (currentWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Staff1)
            {
                LaunchProjectile(magicPrefab, 0, 5);
            }
            else if (currentWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Staff2)
            {
                // 각도를 조금씩 변경하여 화살 두 개를 발사
                LaunchProjectile(magicPrefab, -10, 5);  // 첫 번째 화살은 좌측 각도
                LaunchProjectile(magicPrefab, 10, 5);   // 두 번째 화살은 우측 각도
            }
            else if (currentWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Staff3)
            {
                // 각도를 조금씩 변경하여 화살 세 개를 발사
                LaunchProjectile(magicPrefab, -15, 5);  // 첫 번째 화살은 좌측 각도
                LaunchProjectile(magicPrefab, 0, 5);   // 두 번째 화살은 가운데
                LaunchProjectile(magicPrefab, 15, 5);   // 세 번째 화살은 우측 각도
            }

            yield return new WaitForSeconds(0.3f);  // 이펙트 지속 시간
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
            Vector3 firingPosition = transform.position; // 발사 위치 설정
            GameObject projectile = Instantiate(projectilePrefab, firingPosition, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

            // 마우스 포인터 위치 기준으로 발사 방향 계산
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0; // z-축 값을 0으로 설정

            // 발사 방향 벡터 생성 및 각도 조정
            Vector3 firingDirection = (targetPosition - firingPosition).normalized;
            firingDirection = Quaternion.Euler(0, 0, angle) * firingDirection;

            // Rigidbody2D의 velocity에 발사 방향과 속도 적용
            rb.velocity = firingDirection * speed;

            // 화살의 회전 각도를 발사 방향에 맞추어 설정
            float rotZ = Mathf.Atan2(firingDirection.y, firingDirection.x) * Mathf.Rad2Deg;

            // 화살 스프라이트가 기본적으로 45도 회전된 상태로 제작되었다면, 이를 보정
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