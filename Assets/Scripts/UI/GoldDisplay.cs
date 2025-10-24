using TMPro;
using UnityEngine;

public class GoldDisplay : MonoBehaviour
{
    private CurrencyWallet wallet;   // kéo Player vào đây
    private TMP_Text text;           // kéo GoldText (chính nó) vào

    void Start()
    {
        text = GetComponent<TMP_Text>();

        // 🔍 Find the player in the scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            wallet = player.GetComponent<CurrencyWallet>();
        }

        // 💰 Listen for gold changes
        if (wallet != null)
        {
            wallet.OnGoldChanged.AddListener(OnGoldChanged);
            UpdateText();
        }
        else
        {
            text.text = "0"; // fallback
        }
    }

    void OnDestroy()
    {
        if (wallet) wallet.OnGoldChanged.RemoveListener(OnGoldChanged);
    }

    void OnGoldChanged(int newGold) => UpdateText();

    void UpdateText()
    {
        if (!text || !wallet) return;
        text.text = $"{wallet.gold}";
    }
}
