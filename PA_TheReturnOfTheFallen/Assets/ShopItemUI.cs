using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ShopItem item;
    public NPCShopUI shopUI;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (shopUI.tooltipPanel != null && shopUI.tooltipText != null)
        {
            shopUI.tooltipPanel.SetActive(true);
            shopUI.tooltipText.text = item.description;

            // Opcional: mover o painel para o mouse
            shopUI.tooltipPanel.transform.position = Input.mousePosition;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (shopUI.tooltipPanel != null)
        {
            shopUI.tooltipPanel.SetActive(false);
        }
    }
}
