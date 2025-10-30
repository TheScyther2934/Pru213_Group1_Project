using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public int damage = 5;
    public float attackRange = 1f;
    public LayerMask playerLayer;
    private Collider2D weaponCollider;

    private bool canDamage;
    public Transform attackPoint; // đặt empty object ở vị trí tấn công của quái

    void Awake()
    {
        weaponCollider = GetComponent<Collider2D>();
        if (weaponCollider == null)
        {
            Debug.LogError("EnemyWeapon cần một component Collider2D!", this.gameObject);
            return;
        }

        // Bắt buộc phải là trigger để OnTriggerEnter2D hoạt động
        weaponCollider.isTrigger = true;

        // Tắt collider đi lúc bắt đầu để nó không gây sát thương ngoài ý muốn
        weaponCollider.enabled = false;
    }
    public void EnableDamage()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
    }

    public void DisableDamage()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats stats = other.GetComponent<PlayerStats>();
            if (stats != null)
            {
                Debug.Log($"[DEBUG] Weapon hit player via TRIGGER for {damage}");
                stats.TakeDamage(damage, transform.position);

                // Quan trọng: Tắt hitbox ngay sau khi gây sát thương 1 lần
                // để tránh một nhát chém gây damage nhiều lần.
                DisableDamage();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
