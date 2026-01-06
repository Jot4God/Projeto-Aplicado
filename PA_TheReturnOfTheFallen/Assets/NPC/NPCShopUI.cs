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
    public TMP_SpriteAsset coinSpriteAsset;
    public HotbarController hotbar;

    [Header("Tooltip UI")]
    public GameObject tooltipPanel;
    public TMP_Text tooltipText;

    [Header("Referências do Jogador")]
    public PlayerMoney playerMoney;
    public PlayerArmor playerArmor;
    public PlayerEquipmentUI equipmentUI;
    public PlayerHP playerHP;
    public PlayerController playerController;
    public PlayerMana playerMana;

    [Header("Audio - Compra")]
    public AudioSource buyAudioSource;     // AudioSource (no NPC/UI/Canvas, onde quiseres)
    public AudioClip buySfx;               // Som quando compra
    [Range(0f, 1f)] public float buyVolume = 1f;

    void Start()
    {
        foreach (ShopItem item in itemsForSale)
            item.isSold = false;

        // Garantir AudioSource (se não arrastares no Inspector)
        if (buyAudioSource == null)
            buyAudioSource = GetComponent<AudioSource>();

        if (buyAudioSource != null)
        {
            buyAudioSource.playOnAwake = false;
            buyAudioSource.loop = false;
            buyAudioSource.spatialBlend = 0f; // 2D (som de UI)
            buyAudioSource.dopplerLevel = 0f;
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

        playerMoney.SpendMoney(item.price);
        UpdateMoneyUI();

        // Som de compra (só quando compra mesmo)
        PlayBuySfx();

        // ===== PASSIVOS / EQUIPAMENTOS =====
        if (item.addedArmor > 0 && playerArmor != null)
        {
            playerArmor.EquipArmor(item.addedArmor);
            equipmentUI?.EquipArmorIcon(item.icon, playerArmor.currentArmor);
        }

        if (item.addedSpeed > 0 && playerController != null)
        {
            playerController.speed += item.addedSpeed;
            equipmentUI?.EquipSpeed(playerController.speed, item.icon);
        }

        if (item.addedDashDistance > 0 && playerController != null)
        {
            PlayerDash playerDash = playerController.GetComponent<PlayerDash>();
            if (playerDash != null)
            {
                playerDash.dashDistance += item.addedDashDistance;
                equipmentUI?.EquipDash(playerDash.dashDistance, item.icon);
            }
        }

        if (item.addedRevives > 0 && playerController != null)
        {
            PlayerRespawn respawn = playerController.GetComponent<PlayerRespawn>();
            if (respawn != null)
            {
                respawn.currentRevives += item.addedRevives;
                equipmentUI?.UpdateRevivesUI(respawn.currentRevives, item.icon);
            }
        }

        // ===== CONSUMÍVEIS =====
        bool isConsumable = item.addedHealth > 0 || item.addedMana > 0;

        if (isConsumable && hotbar != null)
        {
            // Adiciona 5 unidades em vez de 1
            for (int i = 0; i < 5; i++)
            {
                hotbar.AddItemToHotbar(item);
            }
        }

        Debug.Log("Compraste: " + item.itemName);
    }

    private void PlayBuySfx()
    {
        if (buyAudioSource == null || buySfx == null) return;
        buyAudioSource.PlayOneShot(buySfx, buyVolume);
    }
}
