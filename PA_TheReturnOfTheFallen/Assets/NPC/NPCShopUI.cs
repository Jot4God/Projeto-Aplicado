using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCShopUI : MonoBehaviour
{
    [Header("Itens disponíveis na loja")]
    public ShopItem[] itemsForSale; // Lista de itens (ScriptableObjects)

    [Header("Referências de UI")]
    public Transform itemContainer; // Painel onde os itens vão aparecer
    public GameObject itemUIPrefab; // Prefab ShopItemUI (modelo do item)
    public TMP_Text playerMoneyText; // Texto que mostra o dinheiro atual

    [Header("Dinheiro do jogador")]
    public int playerMoney = 200;

    void Start()
    {
        LoadItems(); // Gera a loja automaticamente ao abrir
    }

    void LoadItems()
    {
        // Limpa o painel antes de gerar os itens
        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        // Cria um "cartão" (ShopItemUI) para cada item à venda
        foreach (ShopItem item in itemsForSale)
        {
            GameObject newItem = Instantiate(itemUIPrefab, itemContainer);

            // Atualiza os textos e ícones
            newItem.transform.Find("ItemName").GetComponent<TMP_Text>().text = item.itemName;
            newItem.transform.Find("ItemPrice").GetComponent<TMP_Text>().text = item.price + " G";
            newItem.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.icon;

            // Liga o botão de comprar
            Button buyButton = newItem.transform.Find("BuyButton").GetComponent<Button>();
            buyButton.onClick.AddListener(() => BuyItem(item));
        }

        // Atualiza o texto do dinheiro
        playerMoneyText.text = "Moedas: " + playerMoney;
    }

    void BuyItem(ShopItem item)
    {
        if (playerMoney >= item.price)
        {
            playerMoney -= item.price;
            playerMoneyText.text = "Moedas: " + playerMoney;
            Debug.Log("Compraste: " + item.itemName);
        }
        else
        {
            Debug.Log("Dinheiro insuficiente!");
        }
    }
}
