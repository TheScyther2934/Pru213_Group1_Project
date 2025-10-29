using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private bool frozen;
    public Rigidbody2D rb;
    public Animator animator;

    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    Vector2 movement;
    Vector2 lastMoveDirection;
    bool isAttacking = false;

    PlayerStats stats;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>(); // cần add component này trên Player
    }

    void Update()
    {
        if (isAttacking) return;
        if (frozen) return;
        // --- MOVEMENT INPUT ---
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        if (movement.x != 0) movement.y = 0;

        if (movement != Vector2.zero) lastMoveDirection = movement.normalized;

        // --- ANIMATOR PARAMETERS ---
        animator.SetFloat("LastMoveX", lastMoveDirection.x);
        animator.SetFloat("LastMoveY", lastMoveDirection.y);
        animator.SetFloat("MoveX", movement.x);
        animator.SetFloat("MoveY", movement.y);
        animator.SetBool("IsMoving", movement != Vector2.zero);
        

        if (Input.GetKeyDown(KeyCode.Space)) Attack();
    }

    void FixedUpdate()
    {
        if (!isAttacking)
        {
            float moveSpeed = stats != null ? stats.Get(StatType.MoveSpeed) : 5f;
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }

    void Attack()
    {
        if (isAttacking) return;
        isAttacking = true;
        animator.SetTrigger("Attack");
        movement = Vector2.zero;
        animator.SetBool("IsMoving", false);
        animator.SetFloat("LastMoveX", lastMoveDirection.x);
        animator.SetFloat("LastMoveY", lastMoveDirection.y);
    }

    // gọi bởi Animation Event giữa đòn
// ... (các hàm khác giữ nguyên) ...

    // gọi bởi Animation Event giữa đòn
    public void AttackHit()
    {
        Vector2 attackPos = (Vector2)transform.position + lastMoveDirection * attackRange;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, 0.5f, enemyLayers);

        // Giả sử PlayerStats của bạn trả về int. Nếu nó trả về float,
        // bạn cần làm tròn nó trước khi truyền vào EnemyHealth.
        // Ở đây tôi dùng Mathf.RoundToInt để đảm bảo an toàn.
        int dmg = stats != null ? Mathf.RoundToInt(stats.RollAttackDamage()) : 10;

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            // Lấy component EnemyHealth (thay vì "Enemy")
            EnemyHealth enemy = enemyCollider.GetComponent<EnemyHealth>();
            
            if (enemy != null)
            {
                // *** THAY ĐỔI QUAN TRỌNG Ở ĐÂY ***
                // Truyền "transform" (của Player) vào làm tham số thứ hai
                enemy.TakeDamage(dmg, transform);
            }
        }
    }

    // ... (các hàm khác giữ nguyên) ...

    // gọi bởi Animation Event cuối đòn
    public void AttackEnd()
    {
        isAttacking = false;
        movement = Vector2.zero;
        animator.SetBool("IsMoving", false);
        Debug.Log("Attack end trigger");
        animator.SetTrigger("AttackEnd");
    }

    void OnDrawGizmosSelected()
    {
        Vector2 attackPos = (Vector2)transform.position + lastMoveDirection * attackRange;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, 0.5f);
    }

    public void FreezeMovement(bool state)
    {
        frozen = state;
    }

    public void TakeDamage(int amount)
    {
        Debug.Log($"Player dính bẫy, -{amount} HP");
    }
<<<<<<< Updated upstream
=======

    public void ApplyKnockback(Vector2 direction, float force, float duration = 0.25f)
    {
        if (rb == null) return;
        if (isAttacking)
        {
            isAttacking = false;
            animator.SetTrigger("AttackEnd");
        }
        Vector2 dir = direction.normalized;
        isKnockback = true;
        knockbackEndTime = Time.time + duration;
        rb.velocity = dir * force;

        Debug.Log($"ApplyKnockback dir={dir} force={force} duration={duration}");
    }

    public bool IsKnockbackActive
    {
        get { return isKnockback; }
    }
>>>>>>> Stashed changes
}
