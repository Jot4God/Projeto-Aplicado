using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("Ataque Base")]
    public Transform attackPoint;
    public float attackRange = 1.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 20;
    public float attackCooldown = 0.3f;

    [Header("Mana")]
    public int manaCost = 10;
    private PlayerMana playerMana;
    private bool isAttacking = false;

    // ===== ANIMAÇÃO PLAYER =====
    [Header("Animação do Player")]
    public Animator animator;
    public string attackTrigger = "Attack";  // Trigger Sword default

    // ===== ARMA STATE MACHINE =====
    private enum WeaponType { Sword, Spear, Axe }
    private WeaponType currentWeapon = WeaponType.Sword;

    [Header("Sword Stats")]
    public int swordDamage = 20;
    public float swordRange = 1.5f;
    public string swordTrigger = "Attack";

    [Header("Spear Stats")]
    public int spearDamage = 40;
    public float spearRange = 2.8f;
    public string spearTrigger = "AttackSpear";

    [Header("Axe Stats")]
    public int axeDamage = 60;
    public float axeRange = 2.2f;
    public string axeTrigger = "AttackAxe";

    // ===== VISIBLE HITBOX =====
    public float attackPointVisibleDuration = 0.2f;
    private float attackPointTimer = 0f;

    void Start()
    {
        playerMana = GetComponent<PlayerMana>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // === TROCA DE ARMA COM X ===
        if (Input.GetKeyDown(KeyCode.X))
        {
            CycleWeapon();
        }

        // === ATAQUE COM LMB ===
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            if (playerMana != null && playerMana.UseMana(manaCost))
                StartCoroutine(Attack());
            else
                Debug.Log("❌ Sem mana suficiente!");
        }

        // === TIMER DO HITBOX VISÍVEL ===
        if (attackPointTimer > 0)
            attackPointTimer -= Time.deltaTime;
        else
            ShowAttackPointSprite(false);
    }

    // ===========================
    //         ATAQUE
    // ===========================
    IEnumerator Attack()
    {
        isAttacking = true;

        // Trigger correto da arma ativa
        if (animator != null)
            animator.SetTrigger(attackTrigger);

        ShowAttackPointSprite(true);

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            EnemyController ec = enemy.GetComponent<EnemyController>();
            if (ec) ec.TakeDamage(attackDamage);

            BanditAI bandit = enemy.GetComponent<BanditAI>();
            if (bandit) bandit.TakeDamage(attackDamage);

            KnightAI knight = enemy.GetComponent<KnightAI>();
            if (knight) knight.TakeDamage(attackDamage);

            WolfAI wolf = enemy.GetComponent<WolfAI>();
            if (wolf) wolf.TakeDamage(attackDamage);

            CerberusAI cerberus = enemy.GetComponent<CerberusAI>();
            if (cerberus) cerberus.TakeDamage(attackDamage);

            KnightCaptainAI knightcaptain = enemy.GetComponent<KnightCaptainAI>();
            if (knightcaptain) knightcaptain.TakeDamage(attackDamage);

            DemonSlimeAI demonslime = enemy.GetComponent<DemonSlimeAI>();
            if (demonslime) demonslime.TakeDamage(attackDamage);

            GuardsAI guards = enemy.GetComponent<GuardsAI>();
            if (guards) guards.TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }

    // ===========================
    //       TROCA DE ARMA
    // ===========================
    void CycleWeapon()
    {
        if (currentWeapon == WeaponType.Sword)
        {
            currentWeapon = WeaponType.Spear;
            ApplySpearStats();
            Debug.Log("🟡 SPEAR equipada!");
        }
        else if (currentWeapon == WeaponType.Spear)
        {
            currentWeapon = WeaponType.Axe;
            ApplyAxeStats();
            Debug.Log("🪓 AXE equipada!");
        }
        else
        {
            currentWeapon = WeaponType.Sword;
            ApplySwordStats();
            Debug.Log("⚔ SWORD equipada!");
        }
    }

    // ===========================
    //        STATS POR ARMA
    // ===========================
    void ApplySwordStats()
    {
        attackDamage = swordDamage;
        attackRange = swordRange;
        attackTrigger = swordTrigger;
    }

    void ApplySpearStats()
    {
        attackDamage = spearDamage;
        attackRange = spearRange;
        attackTrigger = spearTrigger;
    }

    void ApplyAxeStats()
    {
        attackDamage = axeDamage;
        attackRange = axeRange;
        attackTrigger = axeTrigger;
    }

    // ===========================
    //       HITBOX VISÍVEL
    // ===========================
    void ShowAttackPointSprite(bool show)
    {
        if (attackPoint != null)
        {
            SpriteRenderer sr = attackPoint.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.enabled = show;

            if (show)
                attackPointTimer = attackPointVisibleDuration;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
