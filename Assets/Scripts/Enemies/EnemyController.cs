using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyPathfinding))]
[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(Knockback))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    // Trạng thái của Enemy
    private enum State
    {
        Roaming,
        Chasing,
        Attacking
    }

    [Header("Settings")]
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float roamWaitTime = 2f; // Thời gian chờ trước khi đổi hướng đi lang thang

    [Header("Component References")]
    [SerializeField] private EnemyWeapon weapon; // Kéo thả vũ khí của enemy vào đây

    // Các component được tự động lấy
    private Transform player;
    private EnemyPathfinding pathfinding;
    private EnemyHealth health;
    private Knockback knockback;
    private SpriteRenderer sr;
    private Animator anim;

    // Biến trạng thái
    private State currentState;
    private bool isAttacking;
    private float nextAttackTime;
    private float timeSinceLastRoam;
    private Vector2 roamDirection;

    private void Awake()
    {
        // Tự động lấy các component cần thiết
        pathfinding = GetComponent<EnemyPathfinding>();
        health = GetComponent<EnemyHealth>();
        knockback = GetComponent<Knockback>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

<<<<<<< Updated upstream
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
=======
        // Tự động tìm Player
        FindPlayer();
>>>>>>> Stashed changes
    }

    private void Start()
    {
<<<<<<< Updated upstream
        if (player == null || isAttacking) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= chaseRange && distance > attackRange)
=======
        currentState = State.Roaming;
        nextAttackTime = Time.time;
    }

    private void Update()
    {
        // 1. Kiểm tra điều kiện tiên quyết
        if (player == null)
        {
            FindPlayer(); // Thử tìm lại nếu player bị mất (ví dụ: scene load)
            return; // Không làm gì nếu không có player
        }

        // Nếu quái chết, bị văng, hoặc đang tấn công, nó không thể tự đưa ra quyết định mới
        if (health.IsDead || knockback.gettingKnockedBack || isAttacking)
>>>>>>> Stashed changes
        {
            pathfinding.Stop(); // Đảm bảo dừng di chuyển
            return;
        }

        // 2. Tính toán khoảng cách
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 3. Chuyển đổi trạng thái (State Transitions)
        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            currentState = State.Attacking;
        }
        else if (distanceToPlayer <= chaseRange && distanceToPlayer > attackRange)
        {
            currentState = State.Chasing;
        }
        else if (distanceToPlayer > chaseRange)
        {
            currentState = State.Roaming;
        }

        // 4. Xử lý logic dựa trên trạng thái (State Actions)
        // Logic di chuyển được đặt trong FixedUpdate, ở đây chỉ xử lý logic tức thời
        switch (currentState)
        {
            case State.Roaming:
                HandleRoaming();
                break;
            case State.Chasing:
                HandleChasing();
                break;
            case State.Attacking:
                HandleAttacking();
                break;
        }

        // 5. Cập nhật hình ảnh
        UpdateVisuals();
    }

    private void FixedUpdate()
    {
        // Logic di chuyển chỉ nên chạy trong FixedUpdate để tương thích với vật lý
        // Không di chuyển nếu đang bị knockback, đang tấn công, hoặc đã chết
        if (knockback.gettingKnockedBack || isAttacking || health.IsDead)
        {
            pathfinding.Stop();
            return;
        }

        // Di chuyển theo logic của state hiện tại
        // (Biến `roamDirection` và `dirToPlayer` được tính trong Update)
        if (currentState == State.Roaming)
        {
            pathfinding.Move(roamDirection);
        }
        else if (currentState == State.Chasing)
        {
            Vector2 dirToPlayer = (player.position - transform.position).normalized;
            pathfinding.Move(dirToPlayer);
        }
    }

    // Tìm kiếm Player
    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    // Xử lý logic đi lang thang
    private void HandleRoaming()
    {
        timeSinceLastRoam += Time.deltaTime;
        if (timeSinceLastRoam >= roamWaitTime)
        {
            timeSinceLastRoam = 0;
            roamDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }
    }

    // Xử lý logic đuổi theo
    private void HandleChasing()
    {
        // Logic di chuyển đã được xử lý trong FixedUpdate
        // Logic lật mặt được xử lý trong UpdateVisuals
    }

    // Xử lý logic tấn công
    private void HandleAttacking()
    {
        if (isAttacking) return; // Đang tấn công rồi thì thôi

        pathfinding.Stop();
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;

        // Lật mặt về phía player trước khi đánh
        sr.flipX = (player.position.x < transform.position.x);

        // Kích hoạt animation
        int attackType = Random.Range(1, 3); // Chọn 1 trong 2 kiểu đánh
        anim.SetInteger("AttackIndex", attackType);
        anim.SetBool("IsAttacking", true);

        // Đợi một chút cho animation "vung tay" (điều chỉnh cho khớp với anim của bạn)
        yield return new WaitForSeconds(0.4f);

        // Kích hoạt vũ khí để gây sát thương
        if (weapon != null)
        {
            weapon.EnableDamage();
        }

        // Đợi animation chạy xong (điều chỉnh cho khớp)
        yield return new WaitForSeconds(0.6f);

        // Kết thúc tấn công
        anim.SetBool("IsAttacking", false);
        isAttacking = false;
        nextAttackTime = Time.time + attackCooldown;
    }

    // Cập nhật Animator và lật Sprite
    private void UpdateVisuals()
    {
        // Cập nhật tốc độ cho Animator
        // Dùng MoveDir từ pathfinding thay vì tự tính
        anim.SetFloat("Speed", pathfinding.MoveDir.magnitude);

        // Lật mặt
        // Chỉ lật mặt khi đang di chuyển (để tránh bị giật khi đứng yên)
        if (pathfinding.MoveDir.x != 0)
        {
            sr.flipX = pathfinding.MoveDir.x < 0;
        }
        // Khi đuổi theo, luôn lật mặt đúng
        else if (currentState == State.Chasing)
        {
            sr.flipX = (player.position.x < transform.position.x);
        }
    }
<<<<<<< Updated upstream

}
=======
}
>>>>>>> Stashed changes
