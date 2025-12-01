using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Referências do Inventário")]
    public PlayerInventory playerInventory;

    [Header("UI")]
    public GameObject inventoryPanel;        // Painel que abre/fecha
    public Transform content;                // Content do ScrollView
    public GameObject inventoryItemPrefab;   // Prefab do item do inventário

    void Start()
    {
        inventoryPanel.SetActive(false); // inicia fechado
    }

    void Update()
    {
        // Abrir/Fechar inventário
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);

            if (inventoryPanel.activeSelf)
                RefreshInventory();
        }
    }

    public void RefreshInventory()
    {
        // Limpar itens antigos
        foreach (Transform child in content)
            Destroy(child.gameObject);

        // Criar itens do inventário
        foreach (ShopItem item in playerInventory.items)
        {
            GameObject newItem = Instantiate(inventoryItemPrefab, content);

            // Nome e ícone
            newItem.transform.Find("ItemName").GetComponent<TMP_Text>().text = item.itemName;
            newItem.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.icon;

            // Botão de usar/equipar
            Button useButton = newItem.transform.Find("UseButton").GetComponent<Button>();
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(() => UseItem(item));
        }
    }

    void UseItem(ShopItem item)
    {
        // Aqui apenas debug por enquanto
        Debug.Log("Botão clicado: " + item.itemName);

        // Se for poção, poderia adicionar à PlayerConsumables
        // Se for equipamento, poderia equipar via PlayerArmor, PlayerController, etc
    }
}
