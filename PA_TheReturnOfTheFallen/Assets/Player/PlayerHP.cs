using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public int maxHealth = 100;    
    public int currentHealth;     

    void Awake()
    {
        currentHealth = maxHealth; 
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); 

        Debug.Log("Player recebeu " + damage + " de dano! Vida atual: " + currentHealth);

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
    }

    void Die()
    {
        Debug.Log("Player morreu!");
        // Continua a tua lógica de game over, respawn, etc.
    }
}
