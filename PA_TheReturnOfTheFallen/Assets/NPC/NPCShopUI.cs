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

    void Start()
    {
        LoadItems();
    }

    void LoadItems()
    {
        // Limpa o painel antes de gerar os itens
        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        // Cria os cards de cada item
        foreach (ShopItem item in itemsForSale)
        {
            GameObject newItem = Instantiate(itemUIPrefab, itemContainer);

            newItem.transform.Find("ItemName").GetComponent<TMP_Text>().text = item.itemName;
            newItem.transform.Find("ItemPrice").GetComponent<TMP_Text>().text = item.price + " G";
            newItem.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.icon;

            Button buyButton = newItem.transform.Find("BuyButton").GetComponent<Button>();
            buyButton.onClick.AddListener(() => BuyItem(item));
        }

        UpdateMoneyUI();
    }

    void UpdateMoneyUI()
    {
        playerMoneyText.text = "Moedas: " + playerMoney.currentMoney;
    }

    void BuyItem(ShopItem item)
    {
        if (playerMoney.currentMoney < item.price)
        {
            Debug.Log("Dinheiro insuficiente!");
            return;
        }

        // Subtrai dinheiro
        playerMoney.SpendMoney(item.price);
        UpdateMoneyUI();

        // Aplica ARMOR e mostra o ícone na UI
        if (item.addedArmor > 0 && playerArmor != null)
        {
            playerArmor.EquipArmor(item.addedArmor, item.icon);
            Debug.Log($"Compraste {item.itemName} (+{item.addedArmor} armor)");
        }

        // Aplica VIDA EXTRA
        if (item.addedHealth > 0)
        {
            PlayerHP hp = playerArmor.GetComponent<PlayerHP>();
            if (hp != null)
            {
                hp.maxHealth += item.addedHealth;
                hp.Heal(item.addedHealth);
            }
        }

        // Aplica SPEED EXTRA (se tiver)
        if (item.addedSpeed > 0)
        {
            Debug.Log($"Speed aumentada em {item.addedSpeed}");
        }

        Debug.Log("Compraste: " + item.itemName);
    }
}
