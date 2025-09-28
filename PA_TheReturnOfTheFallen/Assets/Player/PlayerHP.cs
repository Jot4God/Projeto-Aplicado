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

    void Die()
    {
        Debug.Log("Player morreu!");
        // Continuar
    }
}
