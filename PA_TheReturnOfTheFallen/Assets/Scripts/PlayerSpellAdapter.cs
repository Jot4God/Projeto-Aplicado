using UnityEngine;

public class PlayerSpellAdapter : MonoBehaviour
{
    [SerializeField] private PlayerSpell playerSpell; // arrasta o teu PlayerSpell aqui

    private int baseManaCost;
    private float baseCooldown;

    private float manaCostFactor = 1f;   // cumulativo da skilltree (0.9 = -10%)
    private float cooldownFactor = 1f;   // cumulativo

    void Awake()
    {
        if (!playerSpell) playerSpell = GetComponent<PlayerSpell>();
    }

    void Start()
    {
        if (!playerSpell)
        {
            Debug.LogError("[PlayerSpellAdapter] PlayerSpell não encontrado.");
            return;
        }

        baseManaCost = Mathf.Max(0, playerSpell.manaCost);
        baseCooldown = Mathf.Max(0f, playerSpell.spellCooldown);

        // normaliza 1ª vez
        ApplyFactorsToSpell();
    }

    /// <summary>Chamado pela skilltree: define fatores totais (multiplicativos) de custo e cooldown.</summary>
    public void SetSkilltreeSpellFactors(float totalManaCostFactor, float totalCooldownFactor)
    {
        manaCostFactor = Mathf.Max(0f, totalManaCostFactor);
        cooldownFactor = Mathf.Max(0f, totalCooldownFactor);
        ApplyFactorsToSpell();
    }

    private void ApplyFactorsToSpell()
    {
        if (!playerSpell) return;

        // custo final arredondado para int, mínimo 0
        int effectiveCost = Mathf.Max(0, Mathf.RoundToInt(baseManaCost * manaCostFactor));
        float effectiveCooldown = Mathf.Max(0f, baseCooldown * cooldownFactor);

        playerSpell.manaCost = effectiveCost;
        playerSpell.spellCooldown = effectiveCooldown;

        // opcional: Debug
        // Debug.Log($"[SpellAdapter] baseCost={baseManaCost}, baseCD={baseCooldown:F2}, fCost={manaCostFactor:F2}, fCD={cooldownFactor:F2} => cost={effectiveCost}, cd={effectiveCooldown:F2}");
    }

    // Caso queiras mudar base por design (equipamento, upgrades fora da skilltree)
    public void SetBaseSpellParams(int newBaseCost, float newBaseCooldown)
    {
        baseManaCost = Mathf.Max(0, newBaseCost);
        baseCooldown = Mathf.Max(0f, newBaseCooldown);
        ApplyFactorsToSpell();
    }
}
