using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;

    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    Vector2 movement;
    Vector2 lastMoveDirection;
    bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // ⚠️ Don't take input while attacking
        if (isAttacking) return;

        // --- MOVEMENT INPUT ---
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Keep only one dominant direction (avoid diagonal idle confusion)
        if (movement.x != 0) movement.y = 0;

        if (movement != Vector2.zero)
        {
            lastMoveDirection = movement.normalized;
        }

        // --- ANIMATOR PARAMETERS ---
        animator.SetFloat("MoveX", movement.x);
        animator.SetFloat("MoveY", movement.y);
        animator.SetBool("IsMoving", movement != Vector2.zero);

        // --- ATTACK INPUT ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
    }

    void FixedUpdate()
    {
        if (!isAttacking)
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }

    void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");

        // Stop any motion and reset move params
        movement = Vector2.zero;
        animator.SetBool("IsMoving", false);

        // Face last move direction
        animator.SetFloat("LastMoveX", lastMoveDirection.x);
        animator.SetFloat("LastMoveY", lastMoveDirection.y);
    }

    // 🔥 Called by Animation Event
    public void AttackHit()
    {
        Vector2 attackPos = (Vector2)transform.position + lastMoveDirection * attackRange;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, 0.5f, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>()?.TakeDamage(10);
        }
    }

    // 🔚 Called by Animation Event at end
    public void AttackEnd()
    {
        isAttacking = false;
        Debug.Log("Attack end trigger");
        // ✅ Reset to idle cleanly
        movement = Vector2.zero;
        animator.SetBool("IsMoving", false);
        animator.SetTrigger("AttackEnd");
    }

    void OnDrawGizmosSelected()
    {
        Vector2 attackPos = (Vector2)transform.position + lastMoveDirection * attackRange;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, 0.5f);
    }
}
