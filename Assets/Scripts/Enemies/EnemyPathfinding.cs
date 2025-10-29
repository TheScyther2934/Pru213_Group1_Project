using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Knockback))]
public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private LayerMask obstacleLayers; // đặt layer tường (ví dụ: Wall)
    [SerializeField] private float skinWidth = 0.02f;  // mép an toàn tránh dính vào tường
    [SerializeField] private Collider2D movementBounds; // optional: giới hạn AABB của phòng

    private Rigidbody2D rb;
    private Knockback knockback;
    private Collider2D col;

    // Thêm thuộc tính này để Controller có thể đọc
    public Vector2 MoveDir { get; private set; }

    // buffer để tránh GC khi Cast
    private readonly RaycastHit2D[] castHits = new RaycastHit2D[6];

    private void Awake()
    {
        knockback = GetComponent<Knockback>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (knockback.gettingKnockedBack) { return; }

        Vector2 desiredDelta = MoveDir * (moveSpeed * Time.fixedDeltaTime);
        if (desiredDelta == Vector2.zero)
        {
            return;
        }

        // Chuẩn bị filter va chạm với tường (không nhận trigger)
        ContactFilter2D filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = obstacleLayers,
            useTriggers = false
        };

        Vector2 appliedDelta = Vector2.zero;

        // 1) Thử di chuyển toàn bộ delta, nếu bị chặn thì thử trượt theo các trục
        int hitCount = rb.Cast(desiredDelta.normalized, filter, castHits, desiredDelta.magnitude + skinWidth);
        if (hitCount == 0)
        {
            appliedDelta = desiredDelta;
        }
        else
        {
            // Thử theo trục X
            Vector2 deltaX = new Vector2(desiredDelta.x, 0f);
            if (Mathf.Abs(deltaX.x) > 0.0001f)
            {
                int xHits = rb.Cast(Mathf.Sign(deltaX.x) * Vector2.right, filter, castHits, Mathf.Abs(deltaX.x) + skinWidth);
                if (xHits == 0)
                {
                    appliedDelta.x = deltaX.x;
                }
            }

            // Thử theo trục Y (nếu trục X không đi được toàn phần)
            Vector2 deltaY = new Vector2(0f, desiredDelta.y);
            if (Mathf.Abs(deltaY.y) > 0.0001f)
            {
                int yHits = rb.Cast(Mathf.Sign(deltaY.y) * Vector2.up, filter, castHits, Mathf.Abs(deltaY.y) + skinWidth);
                if (yHits == 0)
                {
                    appliedDelta.y = deltaY.y;
                }
            }

            // Nếu cả hai trục đều bị chặn -> đứng yên
        }

        // 2) Áp dụng movement bounds (nếu có) để không rời khỏi phòng (AABB)
        if (movementBounds != null)
        {
            Bounds b = movementBounds.bounds;
            Vector2 target = rb.position + appliedDelta;
            target.x = Mathf.Clamp(target.x, b.min.x, b.max.x);
            target.y = Mathf.Clamp(target.y, b.min.y, b.max.y);
            rb.MovePosition(target);
        }
        else
        {
            rb.MovePosition(rb.position + appliedDelta);
        }
    }

    // Đổi tên từ MoveTo thành Move và nhận vào một hướng (direction)
    public void Move(Vector2 direction)
    {
        MoveDir = direction.normalized;
    }

    // Thêm hàm Stop
    public void Stop()
    {
        MoveDir = Vector2.zero;
    }
}