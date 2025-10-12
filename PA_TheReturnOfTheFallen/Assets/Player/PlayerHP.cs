using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Para manipular a barra de vida

public class PlayerHP : MonoBehaviour
{
    public int maxHealth = 100;    
    public int currentHealth;

    public Slider healthBar; // Slider da UI

    void Awake()
    {
        currentHealth = maxHealth;

        // Inicializa a barra de vida
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log("Player recebeu " + damage + " de dano! Vida atual: " + currentHealth);

        // Atualiza a barra de vida
        if (healthBar != null)
            healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        int oldHealth = currentHealth;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        int actualHealed = currentHealth - oldHealth;
        Debug.Log("Player curado +" + actualHealed + "! Vida atual: " + currentHealth);

        // Atualiza a barra de vida
        if (healthBar != null)
            healthBar.value = currentHealth;
    }

    void Die()
    {
        Debug.Log("Player morreu!");
        SceneManager.LoadScene("GameOver"); 
    }
}
