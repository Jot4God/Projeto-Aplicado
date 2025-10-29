using UnityEngine;

public class PlayerHPAdapter : MonoBehaviour
{
    [SerializeField] private PlayerHP playerHP; // arrasta o teu PlayerHP aqui

    private int baseMaxHealth;
    private float extraFlat;     // vindo da skilltree
    private float extraPercent;  // 0.10 = +10% vindo da skilltree

    void Awake()
    {
        if (!playerHP) playerHP = GetComponent<PlayerHP>();
    }

    void Start()
    {
        if (!playerHP)
        {
            Debug.LogError("[PlayerHPAdapter] PlayerHP não encontrado.");
            return;
        }

        baseMaxHealth = playerHP.maxHealth; // guarda o valor base definido por ti
        // força 1º recálculo para normalizar UI
        ApplyBonusesToHP();
    }

    /// <summary>Chamado pela skilltree: define o bónus total (flat e %) de Enchant Armor.</summary>
    public void SetSkilltreeHPBonuses(float flatBonus, float percentBonus)
    {
        extraFlat = flatBonus;
        extraPercent = percentBonus;
        ApplyBonusesToHP();
    }

    private void ApplyBonusesToHP()
{
    if (!playerHP) return;

    int newMax = Mathf.Max(1, Mathf.RoundToInt(baseMaxHealth * (1f + extraPercent) + extraFlat));
    
    // Atualiza maxHealth
    playerHP.maxHealth = newMax;

    // Mantém a vida atual do jogador, sem sobrescrever
    playerHP.currentHealth = Mathf.Min(playerHP.currentHealth, playerHP.maxHealth);

    // Atualiza apenas maxValue da barra, não value
    if (playerHP.healthBar != null)
    {
        playerHP.healthBar.maxValue = playerHP.maxHealth;
        // Não tocar em healthBar.value aqui!
    }
}

}
