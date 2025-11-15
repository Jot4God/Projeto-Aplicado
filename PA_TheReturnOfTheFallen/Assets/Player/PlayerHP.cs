using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI")]
    public Slider healthBar;
    public Text healthText;

    [Header("Armadura")]
    public PlayerArmor playerArmor; // <-- NOVO

    void Awake()
    {
        currentHealth = maxHealth;

        // Procurar barra se não estiver atribuída
        if (healthBar == null)
        {
            GameObject hb = GameObject.FindGameObjectWithTag("HealthBar");
            if (hb != null) healthBar = hb.GetComponent<Slider>();
        }

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        // Procurar automaticamente o PlayerArmor se faltar
        if (playerArmor == null)
            playerArmor = GetComponent<PlayerArmor>();

        UpdateHealthText();
    }

    void Update()
    {
        if (healthBar != null)
        {
            if (healthBar.maxValue != maxHealth) healthBar.maxValue = maxHealth;
            if (healthBar.value != currentHealth) healthBar.value = currentHealth;
        }

        UpdateHealthText();
    }

    void UpdateHealthText()
    {
        if (healthText != null)
            healthText.text = currentHealth + " / " + maxHealth;
    }

    public void TakeDamage(int damage)
    {
        int armor = (playerArmor != null ? playerArmor.currentArmor : 0);

        int finalDamage = Mathf.Max(damage - armor, 0);

        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"Dano recebido: {damage} | Armor: {armor} | Final: {finalDamage}");

        if (healthBar != null)
            healthBar.value = currentHealth;

        UpdateHealthText();

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        if (healthBar != null) healthBar.value = currentHealth;
        UpdateHealthText();
    }

    void Die()
    {
        GetComponent<PlayerMana>().tempManaCount = 0;
        GetComponent<PlayerMoney>().currentMoney = 0;

        Debug.Log("Player morreu!");
        SceneManager.LoadScene("GameOver");
    }
}
