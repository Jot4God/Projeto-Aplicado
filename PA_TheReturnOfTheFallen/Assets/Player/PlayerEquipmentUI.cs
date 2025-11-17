using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerEquipmentUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text armorText;
    public Image armorIcon;

    public TMP_Text speedText;
    public Image speedIcon;

    public TMP_Text healthText;

    void Start()
    {
        // Nenhum ícone aparece no início
        if (armorIcon != null) armorIcon.gameObject.SetActive(false);
        if (speedIcon != null) speedIcon.gameObject.SetActive(false);
    }

    // ===== ARMOR =====
    public void EquipArmorIcon(Sprite icon, int armorAmount)
    {
        if (armorText != null)
            armorText.text = armorAmount.ToString();

        if (armorIcon != null && icon != null)
        {
            armorIcon.sprite = icon;
            armorIcon.gameObject.SetActive(true);
        }
    }

    // ===== SPEED =====
    public void EquipSpeed(float speedAmount, Sprite icon)
    {
        if (speedText != null)
            speedText.text = Mathf.RoundToInt(speedAmount).ToString();

        if (speedIcon != null && icon != null)
        {
            speedIcon.sprite = icon;
            speedIcon.gameObject.SetActive(true);
        }
    }

    // ===== HEALTH =====
    public void UpdateHealth(int healthAmount)
    {
        if (healthText != null)
            healthText.text = healthAmount.ToString();
    }
}
