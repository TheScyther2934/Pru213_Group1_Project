using TMPro;
using UnityEngine;

public class ShopItemDisplay : MonoBehaviour
{
    public SpriteRenderer iconRenderer; // kéo ItemIcon vào
    public TMP_Text priceText;          // kéo PriceText vào
    public GameObject soldCross;        // optional
    public Transform highlight;         // optional

    public void Show(PermanentStatSO item)
    {
        if (iconRenderer) iconRenderer.sprite = item ? item.icon : null;
        if (priceText)    priceText.text  = item ? item.price.ToString() : "";
        if (soldCross)    soldCross.SetActive(false);
        if (highlight)    highlight.gameObject.SetActive(false);
        gameObject.SetActive(item != null);
    }

    public void SetSold(bool sold)
    {
        if (soldCross) soldCross.SetActive(sold);
        if (priceText) priceText.alpha = sold ? 0.4f : 1f;
        if (iconRenderer) iconRenderer.color = sold ? new Color(1,1,1,0.4f) : Color.white;
    }

    public void SetHighlight(bool on)
    {
        if (highlight) highlight.gameObject.SetActive(on);
    }
}
