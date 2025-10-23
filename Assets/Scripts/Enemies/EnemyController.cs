using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float chaseRange = 5f;
    public float attackRange = 1.2f;
    public float attackCooldown = 2f;

    [Header("References")]
    public Transform player;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private Vector2 moveDir;
    private bool isMoving;
    private bool isAttacking;
    private float nextAttackTime;
    public EnemyWeapon weapon;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= chaseRange && distance > attackRange)
        {
            moveDir = (player.position - transform.position).normalized;
            isMoving = true;
        }
        else
        {
            moveDir = Vector2.zero;
            isMoving = false;
        }

        anim.SetFloat("Speed", moveDir.magnitude);

        // Lật hướng theo player
        if (moveDir.x != 0)
        {
            sr.flipX = moveDir.x < 0;
        }

        // Khi player trong tầm đánh
        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            StartCoroutine(Attack());
        }
    }

    void FixedUpdate()
    {
        if (isMoving && !isAttacking)
        {
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private System.Collections.IEnumerator Attack()
    {
        isAttacking = true;
        isMoving = false;
        anim.SetFloat("Speed", 0);

        int attackType = Random.Range(1, 3);
        anim.SetInteger("AttackIndex", attackType);
        anim.SetBool("IsAttacking", true);

        // Bật collider (nếu không dùng animation event)
        if (weapon != null) weapon.EnableDamage();

        yield return new WaitForSeconds(1f);

        // Tắt collider
        if (weapon != null) weapon.DisableDamage();

        anim.SetBool("IsAttacking", false);
        isAttacking = false;
        nextAttackTime = Time.time + attackCooldown;
    }

    // Gọi từ Animation Event
    public void EnableWeaponDamage()
    {
        if (weapon != null) weapon.EnableDamage();
    }

    public void DisableWeaponDamage()
    {
        if (weapon != null) weapon.DisableDamage();
    }

}
