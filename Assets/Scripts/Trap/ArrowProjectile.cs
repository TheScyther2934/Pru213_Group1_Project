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
    float spawnTime;
    public int damageAmount = 5;
    public float knockbackForce = 400f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // direction: unit vector (top-down)
    public void Launch(Vector2 direction)
    {
        spawnPos = transform.position;
        spawnTime = Time.time;
        rb.velocity = direction.normalized * speed;

        if (sr != null) sr.sortingOrder = orderWhenBehind;

        // compute angle (0 = right), then add sprite offset
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + spriteAngleOffset);
    }

    void Update()
    {
        if (Time.time - spawnTime > lifetime)
        {
            Destroy(gameObject);
            return;
        }

        if (sr != null && sr.sortingOrder == orderWhenBehind)
        {
            float d = Vector3.Distance(spawnPos, transform.position);
            if (d >= distanceToSwitch)
                sr.sortingOrder = orderWhenFront;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.TakeDamage(damageAmount);
        }

        PlayerController controller = other.GetComponentInParent<PlayerController>();
        if (controller != null)
        {
            Vector2 knockDir = rb.velocity.normalized;
            // tùy chỉnh: nếu bạn dùng rb.velocity = dir * force, force nên là một số nhỏ-moderate, ex 5f
            float forceForVelocity = knockbackForce * 0.01f; // nếu knockbackForce gốc bạn đặt lớn, chia nhỏ
            controller.ApplyKnockback(knockDir, forceForVelocity, 0.18f);

            Debug.Log($"Arrow hit player. knockDir={knockDir} applied force={forceForVelocity}");
        }
        else
        {
            Debug.Log("Arrow hit player but PlayerController not found on collider or parents");
        }


        Destroy(gameObject);
    }
}
