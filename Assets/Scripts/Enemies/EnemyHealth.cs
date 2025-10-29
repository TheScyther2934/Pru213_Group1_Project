using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 3;
    [SerializeField] private float deathDestroyTime = 2f;

    private int currentHealth;
    private Knockback knockback;
    private Animator anim;

    public bool IsDead { get; private set; }

    private void Awake()
    {
        knockback = GetComponent<Knockback>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        currentHealth = startingHealth;
        IsDead = false;
    }

    // *** THAY ĐỔI Ở ĐÂY ***
    // Thêm tham số "Transform damageSource"
    public void TakeDamage(int damage, Transform damageSource)
    {
        if (IsDead) return;

        currentHealth -= damage;

        if (currentHealth > 0)
        {
            // Play hurt reaction if an Animator is present
            if (anim != null)
            {
                anim.SetTrigger("Hurt");
            }

            // Bây giờ chúng ta dùng damageSource được truyền vào
            if (damageSource != null)
            {
                knockback.GetKnockedBack(damageSource, 15f);
            }
        }

        DetectDeath();
    }

    private void DetectDeath()
    {
        if (currentHealth <= 0 && !IsDead)
        {
            IsDead = true;

            if (anim != null)
            {
                anim.SetTrigger("Death");
            }

            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;

            Destroy(gameObject, deathDestroyTime);
        }
    }
}