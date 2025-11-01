using TMPro;
using UnityEngine;

/// <summary>
/// Hiển thị icon + giá + hiệu ứng sold/highlight cho từng slot.
/// Không còn dùng info panel tại mỗi slot.
/// </summary>
public class ShopItemDisplay : MonoBehaviour
{
    [Header("Main")]
    public SpriteRenderer iconRenderer; // icon bên trên bệ
    public TMP_Text priceText;          // giá ngay dưới icon

    [Header("FX")]
    public Transform highlight;         // khung highlight
    public GameObject soldCross;        // dấu chéo SOLD (tuỳ chọn)

    ItemSO current;

    public void Show(ItemSO item)
    {
        current = item;
        bool has = item != null;
        gameObject.SetActive(has);

        if (!has)
        {
            if (iconRenderer) iconRenderer.sprite = null;
            if (priceText) priceText.text = "";
            SetSold(false);
            SetHighlight(false);
            return;
        }

        if (iconRenderer) iconRenderer.sprite = item.icon;
        if (priceText)    priceText.text = item.price.ToString();

        SetSold(false);
        SetHighlight(false);
    }

    public void SetSold(bool sold)
    {
        if (soldCross) soldCross.SetActive(sold);
        if (priceText) priceText.alpha = sold ? 0.45f : 1f;
        if (iconRenderer) iconRenderer.color = sold ? new Color(1f, 1f, 1f, 0.45f) : Color.white;
    }

    public void SetHighlight(bool on)
    {
        if (highlight) highlight.gameObject.SetActive(on);
    }
}
