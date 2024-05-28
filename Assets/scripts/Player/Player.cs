using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform weaponHolder; // 무기를 붙일 위치
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

    // 공격 관련 변수
    public float attackCooldown = 1f; // 공격 쿨다운 시간
    private bool isAttacking = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
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
    }

    void FixedUpdate()
    {
        // 플레이어 이동
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 무기와의 충돌 감지
        if (collision.gameObject.CompareTag("Weapon"))
        {
            PickUpWeapon(collision.gameObject);
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
            floatingItem.enabled = false;
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
            case Weapon.WeaponType.Sword:
                currentEffect = swordEffect;
                break;
            case Weapon.WeaponType.Bow:
                currentEffect = bowEffect;
                break;
            case Weapon.WeaponType.Staff:
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
        isAttacking = true;

        // 공격 애니메이션 또는 이펙트 활성화
        if (currentEffect != null)
        {
            currentEffect.SetActive(true);
            // 이펙트를 잠시 활성화 후 비활성화
            yield return new WaitForSeconds(0.3f); // 이펙트 활성화 시간 조절
            currentEffect.SetActive(false);
        }

        // 공격 쿨다운 시간 동안 대기
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;

        // 마우스를 계속 누르고 있으면 재공격
        if (Input.GetMouseButton(0))
        {
            StartCoroutine(Attack());
        }
    }
}