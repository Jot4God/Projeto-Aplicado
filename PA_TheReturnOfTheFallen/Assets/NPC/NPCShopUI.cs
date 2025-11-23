using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCShopUI : MonoBehaviour
{
    [Header("Items do Shop")]
    public ShopItem[] itemsForSale;

    [Header("UI")]
    public Transform itemContainer;
    public GameObject itemUIPrefab;
    public TMP_Text playerMoneyText;
    public TMP_SpriteAsset coinSpriteAsset; // Sprite da moeda

    [Header("Referências do Jogador")]
    public PlayerMoney playerMoney;
    public PlayerArmor playerArmor;
    public PlayerEquipmentUI equipmentUI;
    public PlayerHP playerHP;
    public PlayerController playerController;
    public PlayerMana playerMana;
    public PlayerConsumables playerConsumables; // NOVO: referência ao inventário de consumíveis

    void Start()
    {
        // Reset aos items no início
        foreach (ShopItem item in itemsForSale)
            item.isSold = false;

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

            TMP_Text priceText = newItem.transform.Find("ItemPrice").GetComponent<TMP_Text>();
            priceText.spriteAsset = coinSpriteAsset;
            priceText.text = item.price + " <sprite=0>";

            newItem.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.icon;

            Button buyButton = newItem.transform.Find("BuyButton").GetComponent<Button>();

            if (item.isSold)
            {
                buyButton.interactable = false;
                newItem.transform.Find("SoldText")?.gameObject.SetActive(true);
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
            playerMoneyText.text = playerMoney.currentMoney.ToString();
    }

    void BuyItem(ShopItem item, Button buyButton, GameObject itemUI)
    {
        if (playerMoney.currentMoney < item.price)
        {
            Debug.Log("Dinheiro insuficiente!");
            return;
        }

        // Desconta dinheiro
        playerMoney.SpendMoney(item.price);
        UpdateMoneyUI();

        // Marca como vendido
        item.isSold = true;
        buyButton.interactable = false;
        itemUI.transform.Find("SoldText")?.gameObject.SetActive(true);

        // ARMOR
        if (item.addedArmor > 0 && playerArmor != null)
        {
            playerArmor.EquipArmor(item.addedArmor);
            equipmentUI?.EquipArmorIcon(item.icon, playerArmor.currentArmor);
        }

        // SPEED
        if (item.addedSpeed > 0 && playerController != null)
        {
            playerController.speed += item.addedSpeed;
            equipmentUI?.EquipSpeed(playerController.speed, item.icon);
        }

        // DASH
        if (item.addedDashDistance > 0 && playerController != null)
        {
            PlayerDash playerDash = playerController.GetComponent<PlayerDash>();
            if (playerDash != null)
                playerDash.dashDistance += item.addedDashDistance;
            equipmentUI?.EquipDash(playerDash.dashDistance, item.icon);
        }

        // HEALTH POTION -> adiciona ao inventário de consumíveis
        if (item.addedHealth > 0 && playerConsumables != null)
        {
            playerConsumables.AddHealthPotion(1); // quantidade de poções
        }

        // MANA POTION -> adiciona ao inventário de consumíveis
        if (item.addedMana > 0 && playerConsumables != null)
        {
            playerConsumables.AddManaPotion(1);
        }

        Debug.Log("Compraste: " + item.itemName);
    }
}
