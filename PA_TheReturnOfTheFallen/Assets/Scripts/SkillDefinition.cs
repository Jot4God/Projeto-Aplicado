using UnityEngine;

public enum SkillType
{
    EnchantArmor,
    EnchantWeapon,
    StrengthenSpells
}

[CreateAssetMenu(menuName = "Skills/Skill Definition")]
public class SkillDefinition : ScriptableObject
{
    [field: SerializeField]
    public string Id { get; private set; } = "enchant_armor";

    [field: SerializeField]
    public string DisplayName { get; private set; } = "Enchant Armor";

    [field: SerializeField]
    public SkillType Type { get; private set; } = SkillType.EnchantArmor;

    [field: SerializeField, Min(1)]
    public int MaxRank { get; private set; } = 5;

    [field: SerializeField, Min(1)]
    public int PointCostPerRank { get; private set; } = 1;

    [Header("Enchant Armor")]
    [Tooltip("Aumenta o HP por rank.")]
    public float hpFlatPerRank = 25f;
    public float hpPercentPerRank = 0.05f;

    [Header("Strengthen Spells")]
    [Tooltip("Multiplicador de custo de mana por rank (0.95 = -5% por rank cumulativo).")]
    public float manaCostFactorPerRank = 0.95f;

    [Tooltip("Multiplicador de cooldown dos spells por rank (se não quiseres usar, deixa a 1).")]
    public float cooldownFactorPerRank = 1f;

    [Header("Enchant Weapon")]
    [Tooltip("Aumenta o dano base da arma por rank.")]
    public float weaponDamageFlatPerRank = 5f;

    [Tooltip("Multiplicador de cooldown da arma por rank (0.95 = -5% de cooldown por rank).")]
    public float weaponCooldownFactorPerRank = 0.95f;
}
