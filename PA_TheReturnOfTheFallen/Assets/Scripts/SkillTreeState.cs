using System.Collections.Generic;
using UnityEngine;

public class SkillTreeState : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private PlayerLevel playerLevel;           // arrasta o teu PlayerLevel
    [SerializeField] private PlayerHPAdapter hpAdapter;         // arrasta o PlayerHPAdapter
    [SerializeField] private PlayerSpellAdapter spellAdapter;   // arrasta o PlayerSpellAdapter

    [Header("Skills disponíveis (arrasta os 3 assets)")]
    [SerializeField] private List<SkillDefinition> allSkills = new();

    // Estado interno: skillId -> rank atual
    private readonly Dictionary<string, int> ranks = new();

    // ==== API pública para UI/Debug ====
    public int GetRank(SkillDefinition s) => ranks.TryGetValue(s.Id, out var r) ? r : 0;
    public bool IsMaxed(SkillDefinition s) => GetRank(s) >= s.MaxRank;
    public bool CanRankUp(SkillDefinition s) =>
        s != null && !IsMaxed(s) && playerLevel != null && playerLevel.SkillPoints >= s.PointCostPerRank;

    /// <summary>Tenta subir 1 rank na skill (gasta pontos do PlayerLevel).</summary>
    public bool TryRankUp(SkillDefinition s)
    {
        if (s == null || playerLevel == null) return false;
        if (!CanRankUp(s)) return false;
        if (!playerLevel.TryConsumePoints(s.PointCostPerRank)) return false;

        ranks[s.Id] = GetRank(s) + 1;
        RecomputeAndApplyAllEffects();
        return true;
    }

    /// <summary>Devolve 1 rank da skill e reembolsa pontos (se permitido).</summary>
    public bool TryRefund(SkillDefinition s)
    {
        if (s == null || playerLevel == null) return false;

        int r = GetRank(s);
        if (r <= 0) return false;

        ranks[s.Id] = r - 1;
        playerLevel.RefundPoints(s.PointCostPerRank);
        RecomputeAndApplyAllEffects();
        return true;
    }

    // ==== núcleo: recalcula totais e aplica nos adapters ====
    private void RecomputeAndApplyAllEffects()
    {
        // Enchant Armor
        float totalHPFlat = 0f;
        float totalHPPercent = 0f;

        // Strengthen Spells
        float manaCostFactorTotal = 1f;   // multiplicativo (ex.: 0.95^rank)
        float cooldownFactorTotal = 1f;

        // Enchant Weapon (deixa preparado — quando tiveres o adapter de dano, ligamos)
        float attackFlatTotal = 0f;
        float attackPercentTotal = 0f;

        foreach (var s in allSkills)
        {
            if (!s) continue;
            int r = GetRank(s);
            if (r <= 0) continue;

            switch (s.Type)
            {
                case SkillType.EnchantArmor:
                    totalHPFlat    += s.hpFlatPerRank * r;
                    totalHPPercent += s.hpPercentPerRank * r;
                    break;

                case SkillType.StrengthenSpells:
                    manaCostFactorTotal *= Mathf.Pow(s.manaCostFactorPerRank, r);
                    cooldownFactorTotal *= Mathf.Pow(s.cooldownFactorPerRank, r);
                    break;

                case SkillType.EnchantWeapon:
                    attackFlatTotal    += s.attackFlatPerRank * r;
                    attackPercentTotal += s.attackPercentPerRank * r;
                    break;
            }
        }

        // aplica nos adapters existentes
        if (hpAdapter)    hpAdapter.SetSkilltreeHPBonuses(totalHPFlat, totalHPPercent);
        if (spellAdapter) spellAdapter.SetSkilltreeSpellFactors(manaCostFactorTotal, cooldownFactorTotal);

        // quando tiveres um WeaponAdapter:
        // if (weaponAdapter) weaponAdapter.SetSkilltreeAttackBonuses(attackFlatTotal, attackPercentTotal);
    }

    // ==== (Opcional) Inicialização rápida ====
    private void Awake()
    {
        // garante que não há nulls gritantes
        if (!playerLevel) Debug.LogWarning("[SkillTreeState] PlayerLevel não ligado.");
        if (!hpAdapter) Debug.LogWarning("[SkillTreeState] HPAdapter não ligado.");
        if (!spellAdapter) Debug.LogWarning("[SkillTreeState] SpellAdapter não ligado.");
    }
}
