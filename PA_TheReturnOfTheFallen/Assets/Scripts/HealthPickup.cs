using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 20; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHP ph = other.GetComponent<PlayerHP>();
            if (ph != null && ph.currentHealth < ph.maxHealth)
            {
                int effectiveHeal = Mathf.Min(healAmount, ph.maxHealth - ph.currentHealth);
                ph.Heal(effectiveHeal);

                Destroy(gameObject); 
            }
        }
    }
}
