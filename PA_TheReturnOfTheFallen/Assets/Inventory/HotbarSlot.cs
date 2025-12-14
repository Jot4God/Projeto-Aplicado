using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarSlot : MonoBehaviour
{
    public Image icon;
    public TMP_Text amountText;

    [HideInInspector] public ShopItem currentItem;
    [HideInInspector] public int amount;

    // Highlight
    public Image background;
    public Color normalColor = Color.gray;
    public Color selectedColor = Color.yellow;

    public void SetItem(ShopItem item, int addedAmount)
    {
        currentItem = item;
        amount += addedAmount;

        icon.sprite = item.icon;
        icon.gameObject.SetActive(true);
        UpdateUI();
    }

    public void UseItem(PlayerHP hp, PlayerMana mana)
    {
        if (currentItem == null || amount <= 0) return;

        if (currentItem.addedHealth > 0)
            hp.Heal(currentItem.addedHealth);

        if (currentItem.addedMana > 0)
            mana.RestoreMana(currentItem.addedMana);

        amount--;
        UpdateUI();

        if (amount <= 0)
        {
            currentItem = null;
            icon.gameObject.SetActive(false);
            amountText.text = "";
        }
    }

    void UpdateUI()
    {
        amountText.text = amount > 1 ? amount.ToString() : "";
    }

    public void SetSelected(bool selected)
    {
        if (background != null)
            background.color = selected ? selectedColor : normalColor;
    }
}
