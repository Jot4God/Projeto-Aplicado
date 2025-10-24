using UnityEngine;
using UnityEngine.UI;

public class PlayerMana : MonoBehaviour
{
    [Header("Mana Settings")]
    public int maxMana = 100;
    public int currentMana;

    [Header("Regen Settings")]
    public float regenRate = 10f;
    public float regenDelay = 2f;

    private float lastManaUseTime;
    private float currentManaFloat; // ✅ guarda valor float para regeneração suave

    [Header("UI")]
    public Slider manaBar;

    void Start()
    {
        currentMana = maxMana;
        currentManaFloat = maxMana;

        if (manaBar == null)
        {
            GameObject mb = GameObject.FindGameObjectWithTag("ManaBar");
            if (mb != null)
                manaBar = mb.GetComponent<Slider>();
        }

        if (manaBar != null)
        {
            manaBar.maxValue = maxMana;
            manaBar.value = currentMana;
        }

        lastManaUseTime = -regenDelay;
    }

    void Update()
    {
        RegenerateMana();

        currentMana = Mathf.RoundToInt(currentManaFloat);
        if (manaBar != null)
            manaBar.value = currentMana;
    }

    void RegenerateMana()
    {
        if (lastManaUseTime > 0f && Time.time - lastManaUseTime < regenDelay)
            return;

        if (currentManaFloat < maxMana)
        {
            currentManaFloat += regenRate * Time.deltaTime; // 🔹 suave e contínuo
            currentManaFloat = Mathf.Min(currentManaFloat, maxMana);
        }
    }

    public bool UseMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentManaFloat -= amount;
            lastManaUseTime = Time.time;
            return true;
        }

        return false;
    }

    public void RestoreMana(int amount)
    {
        currentManaFloat += amount;
        currentManaFloat = Mathf.Min(currentManaFloat, maxMana);
    }
}
