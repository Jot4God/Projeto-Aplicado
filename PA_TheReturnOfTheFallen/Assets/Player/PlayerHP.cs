using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI")]
    public Slider healthBar;
    public Text healthText;  // Texto que mostra "vida atual / vida máxima"

    void Awake()
    {
        currentHealth = maxHealth;

        // Tenta encontrar a barra automaticamente
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

        UpdateHealthText();
    }

    void UpdateHealthText()
    {
        if (healthText != null)
            healthText.text = currentHealth + " / " + maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (healthBar != null) healthBar.value = currentHealth;
        UpdateHealthText();

        if (currentHealth <= 0) Die();
    }

    public void Heal(int amount)
    {
        int oldHealth = currentHealth;
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        if (healthBar != null) healthBar.value = currentHealth;
        UpdateHealthText();
    }

    void Die()
    {
        Debug.Log("Player morreu!");
        SceneManager.LoadScene("GameOver");
    }
}
