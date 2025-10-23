using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public int damage = 10;
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
