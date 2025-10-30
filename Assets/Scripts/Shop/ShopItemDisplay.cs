using TMPro;
using UnityEngine;

public class ShopItemDisplay : MonoBehaviour
{
    [Header("Main")]
    public SpriteRenderer iconRenderer;
    public TMP_Text priceText;         // giá ngay dưới icon

    [Header("Info Panel")]
    public GameObject infoPanel;       // bảng thông tin (ẩn/hiện)
    public TMP_Text nameText;
    public TMP_Text descText;
    public TMP_Text infoPriceText;     // giá trong bảng info (nếu muốn)
    public TMP_Text buyHintText;       // dòng "Press B to buy item"

    [Header("FX")]
    public Transform highlight;
    public GameObject soldCross;

    bool infoVisible = false;
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
            ShowInfo(false);
            return;
        }

        if (iconRenderer) iconRenderer.sprite = item.icon;
        if (priceText)    priceText.text = item.price.ToString();

        // điền info panel (mặc định ẩn)
        if (nameText)      nameText.text      = item.displayName;
        if (descText)      descText.text      = item.description;
        if (infoPriceText) infoPriceText.text = item.price.ToString();
        if (buyHintText)   buyHintText.text   = "Press B to buy item";

        SetSold(false);
        ShowInfo(false);
    }

    public void SetSold(bool sold)
    {
        if (soldCross) soldCross.SetActive(sold);
        if (priceText) priceText.alpha = sold ? 0.4f : 1f;
        if (iconRenderer) iconRenderer.color = sold ? new Color(1,1,1,0.4f) : Color.white;
        if (sold) ShowInfo(false);
    }

    public void SetHighlight(bool on)
    {
        if (highlight) highlight.gameObject.SetActive(on);
    }

    public void ShowInfo(bool on)
    {
        infoVisible = on;
        if (infoPanel) infoPanel.SetActive(on);
    }

    public void ToggleInfo() => ShowInfo(!infoVisible);
}
