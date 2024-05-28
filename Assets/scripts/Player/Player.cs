using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform weaponHolder; // ���⸦ ���� ��ġ
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

    // ���� ���� ����
    public float attackCooldown = 1f; // ���� ��ٿ� �ð�
    private bool isAttacking = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
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
    }

    void FixedUpdate()
    {
        // �÷��̾� �̵�
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // ������� �浹 ����
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
            floatingItem.enabled = false;
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
            currentEffect.SetActive(false); // ���� ��Ȱ��ȭ
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        // ���� �ִϸ��̼� �Ǵ� ����Ʈ Ȱ��ȭ
        if (currentEffect != null)
        {
            currentEffect.SetActive(true);
            // ����Ʈ�� ��� Ȱ��ȭ �� ��Ȱ��ȭ
            yield return new WaitForSeconds(0.3f); // ����Ʈ Ȱ��ȭ �ð� ����
            currentEffect.SetActive(false);
        }

        // ���� ��ٿ� �ð� ���� ���
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;

        // ���콺�� ��� ������ ������ �����
        if (Input.GetMouseButton(0))
        {
            StartCoroutine(Attack());
        }
    }
}