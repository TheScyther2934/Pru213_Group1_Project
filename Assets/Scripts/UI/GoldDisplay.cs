using TMPro;
using UnityEngine;

public class GoldDisplay : MonoBehaviour
{
    public CurrencyWallet wallet;   // kéo Player vào đây
    public TMP_Text text;           // kéo GoldText (chính nó) vào

    void Start()
    {
        if (!text) text = GetComponent<TMP_Text>();
        UpdateText();
        if (wallet) wallet.OnGoldChanged.AddListener(OnGoldChanged);
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
