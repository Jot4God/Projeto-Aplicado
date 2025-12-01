using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemUI : MonoBehaviour
{
    public TMP_Text itemNameText;
    public Image itemIconImage;
    public Button useButton;

    public void Setup(ShopItem item, System.Action<ShopItem> onUse)
    {
        itemNameText.text = item.itemName;
        itemIconImage.sprite = item.icon;

        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(() => onUse(item));
    }
}
