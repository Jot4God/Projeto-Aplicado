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

        // manter % de vida atual ao alterar o máximo
        float percent = (playerHP.maxHealth > 0) ? (float)playerHP.currentHealth / playerHP.maxHealth : 1f;

        int newMax = Mathf.Max(1, Mathf.RoundToInt(baseMaxHealth * (1f + extraPercent) + extraFlat));
        playerHP.maxHealth = newMax;

        // repor current mantendo a percentagem
        playerHP.currentHealth = Mathf.Clamp(Mathf.RoundToInt(newMax * percent), 0, newMax);

        // atualizar UI se existir
        if (playerHP.healthBar != null)
        {
            playerHP.healthBar.maxValue = playerHP.maxHealth;
            playerHP.healthBar.value = playerHP.currentHealth;
        }

        // opcional: Debug
        // Debug.Log($"[HPAdapter] base={baseMaxHealth}, flat={extraFlat}, %={extraPercent:P0} => max={playerHP.maxHealth}");
    }
}
