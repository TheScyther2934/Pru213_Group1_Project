using UnityEngine;

public class EnemyDropGold : MonoBehaviour
{
    public GameObject goldPrefab;   // prefab có SpriteRenderer + BoxCollider2D + GoldPickup
    public int minGold = 1, maxGold = 5;

    // gọi hàm này từ Enemy.Die() hoặc OnDestroy (chủ động hơn là gọi trong Die)
    public void Drop()
    {
        if (!goldPrefab) return;
        var g = Instantiate(goldPrefab, transform.position, Quaternion.identity);
        var pick = g.GetComponent<GoldPickup>();
        if (pick) pick.amount = Random.Range(minGold, maxGold + 1);
    }
}
