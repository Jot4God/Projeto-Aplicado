using UnityEngine;
using UnityEngine.UI;

public class PlayerMana : MonoBehaviour
{
    public int maxMana = 100;
    public int currentMana;

    public Slider manaBar; 
    public float regenRate = 10f; 

    void Awake()
    {
        currentMana = maxMana;

        if (manaBar == null)
        {
            GameObject mb = GameObject.FindGameObjectWithTag("ManaBar");
            if (mb != null)
            {
                manaBar = mb.GetComponent<Slider>();
            }
            else
            {
                Debug.LogWarning("⚠️ Nenhum objeto com a tag 'ManaBar' foi encontrado na cena.");
            }
        }

        if (manaBar != null)
        {
            manaBar.maxValue = maxMana;
            manaBar.value = currentMana;
        }
    }

    void Update()
    {
        RegenerateMana();
        if (manaBar != null)
            manaBar.value = currentMana;
    }

    void RegenerateMana()
    {
        if (currentMana < maxMana)
        {
            currentMana += Mathf.RoundToInt(regenRate * Time.deltaTime);
            currentMana = Mathf.Min(currentMana, maxMana);
        }
    }

    public bool UseMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            if (manaBar != null)
                manaBar.value = currentMana;

            Debug.Log("Mana gasta: " + amount + " | Mana atual: " + currentMana);
            return true;
        }

        Debug.Log("❌ Mana insuficiente!");
        return false;
    }

    public void RestoreMana(int amount)
    {
        int oldMana = currentMana;

        currentMana += amount;
        currentMana = Mathf.Min(currentMana, maxMana);

        int actualRestored = currentMana - oldMana;
        Debug.Log("Mana restaurada +" + actualRestored + "! Mana atual: " + currentMana);

        if (manaBar != null)
            manaBar.value = currentMana;
    }
}
