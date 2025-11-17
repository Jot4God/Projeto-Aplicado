using UnityEngine;

public class PlayerArmor : MonoBehaviour
{
    public int currentArmor = 0;

    // Evento que notifica a UI quando muda a armadura
    public delegate void ArmorChanged(int newArmor);
    public event ArmorChanged OnArmorChanged;

    public void EquipArmor(int amount)
    {
        currentArmor = Mathf.Max(0, currentArmor + amount);
        OnArmorChanged?.Invoke(currentArmor);
        Debug.Log("Armor equipada: " + currentArmor);
    }

    public void AddArmor(int amount)
    {
        currentArmor = Mathf.Max(0, currentArmor + amount);
        OnArmorChanged?.Invoke(currentArmor);
    }

    public void SetArmor(int amount)
    {
        currentArmor = amount;
        OnArmorChanged?.Invoke(currentArmor);
    }
}
