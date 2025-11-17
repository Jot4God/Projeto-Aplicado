using UnityEngine;

public class ManaPoolZone3D : MonoBehaviour
{
    public int maxUses = 5;
    public int manaAmount = 10;      // ignora isto se quiseres (a pool agora aumenta max mana)
    public float cooldown = 1f;

    private int currentUses;
    private float lastHealTime = 0f;

    void Start()
    {
        currentUses = maxUses;
    }

    void OnTriggerStay(Collider other)
    {
        if (currentUses <= 0) return;
        if (Time.time - lastHealTime < cooldown) return;

        PlayerMana playerMana = other.GetComponent<PlayerMana>();
        if (playerMana != null)
        {
            // --- AUMENTA O MÁXIMO DE MANA EM 10 ---
            playerMana.maxMana += 10;

            // Atualiza UI imediatamente
            if (playerMana.manaBar != null)
                playerMana.manaBar.maxValue = playerMana.maxMana;

            // Enche a mana para o novo máximo
            playerMana.RestoreMana(playerMana.maxMana);

            // Contabiliza uso da pool
            currentUses--;
            lastHealTime = Time.time;

            if (currentUses <= 0)
                DisablePool();
        }
    }

    void DisablePool()
    {
        Debug.Log("Mana Pool esgotada!");

        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
            rend.material.color = Color.gray;

        GetComponent<Collider>().enabled = false;
    }
}
