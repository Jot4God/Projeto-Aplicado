using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCShopUI : MonoBehaviour
{
    public ShopItem[] itemsForSale;
    public Transform itemContainer;
    public GameObject itemUIPrefab;
    public TMP_Text playerMoneyText;

    [Header("Referências do jogador")]
    public PlayerMoney playerMoney;
    public PlayerArmor playerArmor;
    public PlayerEquipmentUI equipmentUI;
    public PlayerHP playerHP;
    public PlayerController playerController;

    void Start()
{
    // Reseta todos os ShopItems para não vendidos
    foreach (ShopItem item in itemsForSale)
    {
        item.isSold = false;
    }

    LoadItems();
    UpdateMoneyUI();
}


    void LoadItems()
    {
        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        foreach (ShopItem item in itemsForSale)
        {
            GameObject newItem = Instantiate(itemUIPrefab, itemContainer);
            newItem.transform.Find("ItemName").GetComponent<TMP_Text>().text = item.itemName;
            newItem.transform.Find("ItemPrice").GetComponent<TMP_Text>().text = item.price + " G";
            newItem.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.icon;

            Button buyButton = newItem.transform.Find("BuyButton").GetComponent<Button>();
            
            // Checa se o item já foi vendido
            if (item.isSold)
            {
                buyButton.interactable = false;
                newItem.transform.Find("SoldText")?.gameObject.SetActive(true); // se tiver um Text ou imagem "X"
            }
            else
            {
                buyButton.onClick.AddListener(() => BuyItem(item, buyButton, newItem));
            }
        }
    }

    void UpdateMoneyUI()
    {
        if (playerMoneyText != null)
            playerMoneyText.text = "Moedas: " + playerMoney.currentMoney;
    }

    void BuyItem(ShopItem item, Button buyButton, GameObject itemUI)
    {
        if (playerMoney.currentMoney < item.price)
        {
            Debug.Log("Dinheiro insuficiente!");
            return;
        }

        // Subtrai dinheiro
        playerMoney.SpendMoney(item.price);
        UpdateMoneyUI();

        // Marca como vendido
        item.isSold = true;
        buyButton.interactable = false;

        // Mostra X ou Sold
        itemUI.transform.Find("SoldText")?.gameObject.SetActive(true);

        // ===== ARMOR =====
        if (item.addedArmor > 0 && playerArmor != null)
        {
            playerArmor.EquipArmor(item.addedArmor);
            equipmentUI?.EquipArmorIcon(item.icon, playerArmor.currentArmor);
        }

        // ===== HEALTH =====
        if (item.addedHealth > 0 && playerHP != null)
        {
            // Não aumentamos maxHealth, só curamos
            playerHP.Heal(item.addedHealth);        
            equipmentUI?.UpdateHealth(playerHP.currentHealth);
        }

        // ===== SPEED =====
        if (item.addedSpeed > 0 && playerController != null)
        {
            playerController.speed += item.addedSpeed;
            equipmentUI?.EquipSpeed(playerController.speed, item.icon);
        }

        Debug.Log("Compraste: " + item.itemName);
    }
}
