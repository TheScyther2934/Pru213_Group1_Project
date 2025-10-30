using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ArrowProjectile : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 14f;
    public float lifetime = 5f;

    [Header("Visual / rotation")]
    [Tooltip("Nếu sprite của bạn vẽ mặc định hướng 'down' (nhìn xuống), set = 90.\n" +
             "Nếu sprite hướng 'right' set = 0. 'up' => -90, 'left' => 180.")]
    public float spriteAngleOffset = 0f;

    [Header("Order In Layer Switch (visual)")]
    public int orderWhenBehind = 0;
    public int orderWhenFront = 10;
    public float distanceToSwitch = 0.25f;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Vector3 spawnPos;
    //float spawnTime;
    public int damageAmount = 5;
    public float knockbackForce = 400f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        // Thay vì dùng Update để check lifetime, chúng ta dùng Invoke
        // Nó sẽ tự động gọi hàm ReturnToPool sau 'lifetime' giây. Hiệu năng tốt hơn.
        Invoke(nameof(ReturnToPool), lifetime);
    }

    // direction: unit vector (top-down)
    public void Launch(Vector2 direction)
    {
        spawnPos = transform.position;
        //spawnTime = Time.time;
        rb.velocity = direction.normalized * speed;

        if (sr != null) sr.sortingOrder = orderWhenBehind;

        // compute angle (0 = right), then add sprite offset
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + spriteAngleOffset);
    }

    void Update()
    {
        if (sr != null && sr.sortingOrder == orderWhenBehind)
        {
            float d = Vector3.Distance(spawnPos, transform.position);
            if (d >= distanceToSwitch)
                sr.sortingOrder = orderWhenFront;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            ReturnToPool(); // Trả về pool thay vì Destroy
            return;
        }
        if (!other.CompareTag("Player")) return;

        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.TakeDamage(damageAmount, transform.position);
        }
        ReturnToPool();
    }
    private void ReturnToPool()
    {
        // Quan trọng: Hủy Invoke để tránh việc nó được gọi lại sau khi đã va chạm
        CancelInvoke(nameof(ReturnToPool));
        // Tắt object đi thay vì hủy
        gameObject.SetActive(false);
    }
}
