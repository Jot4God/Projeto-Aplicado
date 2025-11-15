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
    public PlayerMoney playerMoney;   // Já ligado no Inspector
    public PlayerArmor playerArmor;   // Arrasta o Player com PlayerArmor aqui

    void Start()
    {
        LoadItems();
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
        if (playerMoney.currentMoney >= item.price)
        {
            playerMoney.SpendMoney(item.price);
            UpdateMoneyUI();

            // Verifica se é um item de armadura
            if (item.itemName.Contains("Armor"))
            {
                if (playerArmor != null)
                {
                    playerArmor.AddArmor(20); // aumenta 20 de armadura
                    Debug.Log("Compraste: " + item.itemName + " (+20 Armor)");
                }
            }
            else
            {
                Debug.Log("Compraste: " + item.itemName);
            }
        }
        else
        {
            Debug.Log("Dinheiro insuficiente!");
        }
    }
}
