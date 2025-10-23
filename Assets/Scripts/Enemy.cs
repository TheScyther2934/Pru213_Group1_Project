using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 20f;
    public float knockbackForce = 3f;
    public float deathDelay = 1f; // Thời gian chờ trước khi xóa enemy sau khi chết

    private Animator anim;
    private Rigidbody2D rb;
    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        anim?.SetTrigger("HurtTrigger");
        StartCoroutine(KnockbackRoutine());

        if (health <= 0f) Die();
    }

    IEnumerator KnockbackRoutine()
    {
        Vector2 knockDir = (transform.position - FindObjectOfType<PlayerController>().transform.position).normalized;
        float knockTime = 0.15f;
        float knockSpeed = 5f;

        float t = 0;
        while (t < knockTime)
        {
            transform.Translate(knockDir * knockSpeed * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }
    }


    public void Die(GameObject killer = null)
    {
        if (isDead) return;
        isDead = true;

        // Bật animation chết
        anim?.SetBool("IsDead", true);

        // Ngừng mọi chuyển động vật lý
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; // không bị đẩy nữa
            rb.simulated = false;
        }

        // Drop vàng (nếu có)
        GetComponent<EnemyDropGold>()?.DropToPlayer(killer ?? FindObjectOfType<PlayerController>().gameObject);

        // Xóa enemy sau khi animation chạy xong
        Destroy(gameObject, deathDelay);
    }
}
