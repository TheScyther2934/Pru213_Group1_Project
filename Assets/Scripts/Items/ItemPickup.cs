using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    public Consumable item;
    public bool destroyOnUse = true;

    void Awake()
    {
        // tự thêm BoxCollider2D nếu thiếu
        var col = GetComponent<Collider2D>();
        if (col == null) col = gameObject.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!item) return;
        if (!other.CompareTag("Player")) return;

        var stats = other.GetComponent<PlayerStats>();
        if (!stats) return;

        item.UseOn(stats);
        if (destroyOnUse) Destroy(gameObject);
    }
}
