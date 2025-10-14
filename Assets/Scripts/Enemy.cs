using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 20f;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f) Die();
    }

    public void Die(GameObject killer = null)
    {
        if (killer)
            GetComponent<EnemyDropGold>()?.DropToPlayer(killer);
        else
            GetComponent<EnemyDropGold>()?.DropToPlayer(FindObjectOfType<PlayerController>().gameObject);
        Destroy(gameObject);
    }

}
