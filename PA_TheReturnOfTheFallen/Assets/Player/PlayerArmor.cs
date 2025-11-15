using UnityEngine;
using TMPro;

public class PlayerArmor : MonoBehaviour
{
    public int currentArmor = 0;       // Armadura atual
    public TMP_Text armorText;         // Texto na UI que mostra a armadura

    void Start()
    {
        UpdateArmorUI();
    }

    public void AddArmor(int amount)
    {
        currentArmor += amount;
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
}
