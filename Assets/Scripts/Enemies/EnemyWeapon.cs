using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public int damage = 10;
    public float attackRange = 1f;
    public LayerMask playerLayer;

    private bool canDamage;
    public Transform attackPoint; // đặt empty object ở vị trí tấn công của quái

    public void EnableDamage()
    {
        canDamage = true;
        TryHitPlayer();
    }

    public void DisableDamage() => canDamage = false;

    private void TryHitPlayer()
    {
        if (!canDamage) return;

        // tạo 1 vùng tròn kiểm tra có Player không
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
        if (hitPlayer != null)
        {
            var stats = hitPlayer.GetComponent<PlayerStats>();
            if (stats)
            {
                Debug.Log($"[DEBUG] Enemy hit player for {damage}");
                stats.TakeDamage(damage);
            }
        }
        else
        {
            Debug.Log("[DEBUG] Attack missed");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
