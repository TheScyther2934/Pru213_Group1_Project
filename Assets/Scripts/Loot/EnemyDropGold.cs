using UnityEngine;

public class EnemyDropGold : MonoBehaviour
{
    public int minGold = 1, maxGold = 5;

    // Gọi khi enemy chết
    public void DropToPlayer(GameObject killer)
    {
        var wallet = killer.GetComponent<CurrencyWallet>();
        if (wallet != null)
        {
            int gold = Random.Range(minGold, maxGold + 1);
            wallet.AddGold(gold);
            Debug.Log($"+{gold} vàng!");
        }
    }
}
