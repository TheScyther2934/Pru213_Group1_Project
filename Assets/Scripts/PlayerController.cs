using System.Collections;
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
    private bool isInvulnerable = false;
    private Coroutine takeDamageCoroutine;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>(); // cần add component này trên Player
    }

    void Update()
    {
        if (isAttacking || frozen || isInvulnerable)
        {
            movement = Vector2.zero; // Đảm bảo nhân vật không trôi đi
            return;
        }
        // --- MOVEMENT INPUT ---
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();

        if (movement != Vector2.zero)
        {
            lastMoveDirection = movement;
        }

        // --- ANIMATOR PARAMETERS ---
        animator.SetFloat("LastMoveX", lastMoveDirection.x);
        animator.SetFloat("LastMoveY", lastMoveDirection.y);
        animator.SetFloat("MoveX", movement.x);
        animator.SetFloat("MoveY", movement.y);
        animator.SetBool("IsMoving", movement.sqrMagnitude > 0.01f);



        if (Input.GetKeyDown(KeyCode.Space)) Attack();
    }

    void FixedUpdate()
    {
        // Khi bị choáng/knockback, Coroutine sẽ xử lý vận tốc, nên chúng ta không di chuyển ở đây
        if (isAttacking || isInvulnerable)
        {
            // Có thể muốn dừng hẳn nhân vật nếu không có knockback
            if (!isInvulnerable) rb.velocity = Vector2.zero;
            return;
        }

        float currentMoveSpeed = stats != null ? stats.Get(StatType.MoveSpeed) : 5f;
        rb.MovePosition(rb.position + movement.normalized * currentMoveSpeed * Time.fixedDeltaTime);
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
    public void AttackHit()
    {
        Vector2 attackPos = (Vector2)transform.position + lastMoveDirection * attackRange;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, 0.5f, enemyLayers);

        float dmg = stats != null ? stats.RollAttackDamage() : 10f;

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>()?.TakeDamage(dmg);
        }
    }

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

    public void HandleTakeDamage(Vector2 knockbackDirection, float knockbackForce)
    {
        if (isInvulnerable) return;

        // Dừng coroutine cũ nếu có để reset lại trạng thái choáng
        if (takeDamageCoroutine != null)
        {
            StopCoroutine(takeDamageCoroutine);
        }
        takeDamageCoroutine = StartCoroutine(TakeDamageRoutine(knockbackDirection, knockbackForce));
    }

    private IEnumerator TakeDamageRoutine(Vector2 knockbackDirection, float knockbackForce)
    {
        // Nếu bị choáng, chúng ta phải hủy bỏ trạng thái "đang tấn công"
        // để tránh bị kẹt nếu đòn đánh bị ngắt ngang.
        isAttacking = false;

        // Bắt đầu trạng thái "choáng" và "bất tử"
        isInvulnerable = true;
        animator.SetTrigger("Hurt");

        // Áp dụng lực đẩy lùi
        rb.velocity = Vector2.zero; // Xóa vận tốc cũ
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        // Đợi một khoảng thời gian ngắn
        yield return new WaitForSeconds(0.4f);

        // Kết thúc trạng thái
        rb.velocity = Vector2.zero; // Dừng hẳn sau khi văng đi
        isInvulnerable = false;
        takeDamageCoroutine = null;
    }
}
