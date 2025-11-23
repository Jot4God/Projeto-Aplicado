using UnityEngine;
using UnityEngine.UI;

public class PlayerConsumables : MonoBehaviour
{
    [Header("Referências")]
    public PlayerHP playerHP;
    public PlayerMana playerMana;

    [Header("UI")]
    public Image healthPotionIcon;
    public Image manaPotionIcon;

    [HideInInspector] public int healthPotions = 0;
    [HideInInspector] public int manaPotions = 0;

    void Update()
    {
        // Atalhos para usar poções
        if (Input.GetKeyDown(KeyCode.Alpha1)) // tecla 1 para HP
            UseHealthPotion();
        if (Input.GetKeyDown(KeyCode.Alpha2)) // tecla 2 para Mana
            UseManaPotion();
    }

    public void AddHealthPotion(int amount)
    {
        healthPotions += amount;
        if (healthPotionIcon != null)
            healthPotionIcon.gameObject.SetActive(true);
    }

    public void AddManaPotion(int amount)
    {
        manaPotions += amount;
        if (manaPotionIcon != null)
            manaPotionIcon.gameObject.SetActive(true);
    }

    void UseHealthPotion()
    {
        if (healthPotions > 0 && playerHP != null)
        {
            playerHP.Heal(50); // valor da poção, pode ser do ShopItem
            healthPotions--;
            if (healthPotions <= 0 && healthPotionIcon != null)
                healthPotionIcon.gameObject.SetActive(false);
        }
    }

    void UseManaPotion()
    {
        if (manaPotions > 0 && playerMana != null)
        {
            playerMana.RestoreMana(50); // valor da poção, pode ser do ShopItem
            manaPotions--;
            if (manaPotions <= 0 && manaPotionIcon != null)
                manaPotionIcon.gameObject.SetActive(false);
        }
    }
}
