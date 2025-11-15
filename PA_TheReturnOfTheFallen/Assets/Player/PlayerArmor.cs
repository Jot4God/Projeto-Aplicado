using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerArmor : MonoBehaviour
{
    public int currentArmor = 0;       // Armadura atual
    public TMP_Text armorText;         // Texto da UI (ex: "20")
    public Image armorIconUI;          // Ícone da armadura no HUD

    private Sprite equippedArmorIcon;  // Guarda o ícone atual

    void Start()
    {
        if (armorIconUI != null)
            armorIconUI.gameObject.SetActive(false); // começa escondido
        UpdateArmorUI();
    }

    public void EquipArmor(int amount, Sprite icon)
    {
        currentArmor = Mathf.Max(0, currentArmor + amount);
        equippedArmorIcon = icon;

        UpdateArmorUI();
        UpdateArmorIcon();
    }

    public void AddArmor(int amount)
    {
        currentArmor = Mathf.Max(0, currentArmor + amount);
        UpdateArmorUI();
    }

    public void SetArmor(int amount)
    {
        currentArmor = amount;
        UpdateArmorUI();
    }

    void UpdateArmorUI()
    {
        if (armorText != null)
            armorText.text = currentArmor.ToString();
    }

    void UpdateArmorIcon()
    {
        if (armorIconUI != null && equippedArmorIcon != null)
        {
            armorIconUI.sprite = equippedArmorIcon;
            armorIconUI.gameObject.SetActive(true); // garante que fica visível
        }
    }
}
