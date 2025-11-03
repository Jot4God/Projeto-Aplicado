using UnityEngine;

public class LimitedSpawnPoint : MonoBehaviour
{
    [Header("Mana Pool Settings")]
    public int maxUses = 3;           // Quantas vezes pode ser usado
    public int manaAmount = 20;       // Mana que o jogador recebe por uso

    private int currentUses;

    void Start()
    {
        currentUses = maxUses;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (currentUses <= 0) return;

        PlayerMana playerMana = other.GetComponent<PlayerMana>();
        if (playerMana != null)
        {
            playerMana.RestoreMana(manaAmount);
            currentUses--;

            if (currentUses <= 0)
            {
                DisablePool();
            }
        }
    }

    void DisablePool()
    {
        Debug.Log("Mana Pool esgotada!");
        gameObject.SetActive(false); // desativa o pool
    }
}
