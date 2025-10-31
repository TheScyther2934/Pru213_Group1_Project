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
    private ContactPoint2D[] contacts = new ContactPoint2D[4];

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        //if (player == null)
        //    player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null)
        {
            // Tìm đối tượng có tag "Player"
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                // Nếu không tìm thấy, đứng yên và không làm gì cả
                isMoving = false;
                moveDir = Vector2.zero;
                anim.SetFloat("Speed", 0);
                return; // Thoát khỏi hàm Update cho frame này
            }
        }

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
        if (moveDir.x > 0.01f) // Di chuyển sang phải
        {
            transform.localScale = new Vector3(1, 1, 1); // Scale mặc định
        }
        else if (moveDir.x < -0.01f) // Di chuyển sang trái
        {
            transform.localScale = new Vector3(-1, 1, 1); // Lật theo trục X
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
            Vector2 finalMoveDir = moveDir; // Hướng di chuyển mặc định

            // Lấy tất cả các điểm va chạm hiện tại của Rigidbody
            int contactCount = rb.GetContacts(contacts);

            // Duyệt qua các điểm va chạm (nếu có)
            for (int i = 0; i < contactCount; i++)
            {
                // Chỉ quan tâm đến va chạm với tường
                if (contacts[i].collider.CompareTag("Wall"))
                {
                    // Lấy vector pháp tuyến (chỉa thẳng ra từ bề mặt tường)
                    Vector2 wallNormal = contacts[i].normal;

                    // Dùng Tích vô hướng (Dot product) để kiểm tra xem chúng ta đang đi VÀO hay ĐI RA khỏi tường
                    // Nếu kết quả < 0, tức là góc giữa hướng đi và pháp tuyến > 90 độ -> Đang đi vào tường
                    if (Vector2.Dot(moveDir, wallNormal) < 0)
                    {
                        // Áp dụng logic trượt tường
                        finalMoveDir = moveDir - Vector2.Dot(moveDir, wallNormal) * wallNormal;
                        finalMoveDir.Normalize();
                        break; // Thoát khỏi vòng lặp khi đã tìm thấy va chạm với tường
                    }
                }
            }

            // Gán vận tốc theo hướng đi cuối cùng (hoặc là hướng trượt, hoặc là hướng gốc)
            rb.velocity = finalMoveDir * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
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

        // Đợi một chút để animation vung vũ khí
        yield return new WaitForSeconds(0.3f); // Tùy chỉnh thời gian này cho khớp với animation

        // Bật "vùng nguy hiểm" của vũ khí lên
        if (weapon != null) weapon.EnableDamage();

        // Đợi thêm một chút cho "vùng nguy hiểm" tồn tại
        yield return new WaitForSeconds(0.2f); // Đây là "damage window"

        // Tắt "vùng nguy hiểm" đi
        if (weapon != null) weapon.DisableDamage();

        // Đợi phần còn lại của animation (nếu có)
        // Tổng thời gian chờ nên khớp với độ dài animation tấn công
        yield return new WaitForSeconds(0.5f);
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