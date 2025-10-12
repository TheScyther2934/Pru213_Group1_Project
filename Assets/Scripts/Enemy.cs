using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 20f;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f) Die();
    }

    void Die() => Destroy(gameObject);
}
