using UnityEngine;

public class PlayerWeaponAdapter : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private PlayerAttack playerAttack;

    [Header("Base da Arma (sem skilltree)")]
    [SerializeField] private int baseDamage;
    [SerializeField] private float baseCooldown = 0.3f;

    [Header("Bónus do Skilltree")]
    [SerializeField] private float skillFlatDamageBonus = 0f;
    [SerializeField] private float skillCooldownFactor = 1f; // 1 = igual, <1 = cooldown mais baixo

    private void Awake()
    {
        if (!playerAttack)
            playerAttack = GetComponent<PlayerAttack>();
    }

    /// <summary>Chamado pelo PlayerAttack quando mudas de arma.</summary>
    public void SetBaseWeaponStats(int damage, float cooldown)
    {
        baseDamage = damage;
        baseCooldown = cooldown;
        ApplyFinalStats();
    }

    /// <summary>Chamado pelo SkillTreeState quando re-calcula os bónus.</summary>
    public void SetSkilltreeWeaponBonuses(float flatDamageBonus, float cooldownFactor)
    {
        skillFlatDamageBonus = flatDamageBonus;
        skillCooldownFactor = cooldownFactor;
        ApplyFinalStats();
    }

    private void ApplyFinalStats()
    {
        if (playerAttack == null) return;

        // dano final: base + bónus fixo
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(baseDamage + skillFlatDamageBonus));

        // cooldown final: base * fator
        float finalCooldown = Mathf.Max(0.01f, baseCooldown * skillCooldownFactor);

        playerAttack.attackDamage = finalDamage;
        playerAttack.attackCooldown = finalCooldown;
    }
}
