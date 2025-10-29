using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public int damage = 10;
<<<<<<< Updated upstream
    private bool canDamage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canDamage) return;
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>()?.TakeDamage(damage);
        }
    }

    // Gọi từ animation event
    public void EnableDamage() => canDamage = true;
    public void DisableDamage() => canDamage = false;
}
=======
    public float attackRange = 1f;
    public LayerMask playerLayer;

    // 'attackPoint' nên là một Empty Object con của Enemy
    // đặt ở vị trí vung vũ khí
    public Transform attackPoint;

    // Biến này không thực sự cần thiết với logic hiện tại
    // private bool canDamage; 

    // Hàm này được gọi bởi EnemyController (hoặc Animation Event)
    // để thực hiện kiểm tra sát thương
    public void EnableDamage()
    {
        // canDamage = true; // Không cần thiết nếu bạn gọi TryHitPlayer ngay
        TryHitPlayer();
    }

    // Hàm này có thể dùng nếu bạn muốn vũ khí "bật" trong 1 khoảng thời gian
    // Nhưng với logic hiện tại (check tức thời) thì không cần
    // public void DisableDamage() => canDamage = false;

    private void TryHitPlayer()
    {
        if (attackPoint == null)
        {
            Debug.LogError("Chưa gán AttackPoint cho EnemyWeapon!");
            return;
        }

        // Tạo 1 vùng tròn kiểm tra có Player không
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);

        if (hitPlayer != null)
        {
            // Thử lấy component PlayerStats (hoặc tên script máu của player)
            var stats = hitPlayer.GetComponent<PlayerStats>(); // ĐỔI TÊN NẾU CẦN
            if (stats)
            {
                Debug.Log($"[DEBUG] Enemy hit player for {damage}");
                stats.TakeDamage(damage); // Gọi hàm nhận sát thương của Player
            }
        }
        else
        {
            Debug.Log("[DEBUG] Attack missed");
        }
    }

    // Vẽ Gizmos để dễ dàng chỉnh sửa trong Editor
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
>>>>>>> Stashed changes
