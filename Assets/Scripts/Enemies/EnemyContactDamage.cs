using System.Collections.Generic;
using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int contactDamage = 5;
    [SerializeField] private float hitCooldown = 0.5f;

    [Header("Knockback Settings")] 
    [SerializeField] private float knockbackForce = 12f;
    [SerializeField] private float knockbackDuration = 0.25f;

    private readonly Dictionary<GameObject, float> lastHitTimeByTarget = new Dictionary<GameObject, float>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHit(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryHit(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Allow repeated damage with cooldown while staying in contact
        TryHit(other.gameObject);
    }

    private void TryHit(GameObject target)
    {
        if (target == null) return;
        if (!target.CompareTag("Player")) return;

        float effectiveCooldown = hitCooldown;
        // Đảm bảo thời gian hồi tối thiểu dài hơn thời gian knockback một chút
        if (knockbackDuration + 0.05f > effectiveCooldown)
        {
            effectiveCooldown = knockbackDuration + 0.05f;
        }

        float lastTime;
        if (lastHitTimeByTarget.TryGetValue(target, out lastTime))
        {
            if (Time.time - lastTime < effectiveCooldown) return;
        }

        var playerController = target.GetComponent<PlayerController>();
        if (playerController != null && playerController.IsKnockbackActive)
        {
            // Đang bị knockback thì không gây thêm hit để tránh khóa di chuyển kéo dài
            return;
        }

        lastHitTimeByTarget[target] = Time.time;

        var playerStats = target.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.TakeDamage(contactDamage);
        }

        if (playerController != null)
        {
            Vector2 dir = (target.transform.position - transform.position).normalized;
            playerController.ApplyKnockback(dir, knockbackForce, knockbackDuration);
        }
    }
}


