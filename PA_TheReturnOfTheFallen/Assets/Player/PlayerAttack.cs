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
    public string attackTrigger = "Attack";

    // ===== SOM DE ATAQUE =====
    [Header("Som de Ataque")]
    public AudioSource audioSource;
    public AudioClip attackSound;

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

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // === TROCA DE ARMA ===
        if (Input.GetKeyDown(KeyCode.X))
        {
            CycleWeapon();
        }

        // === ATAQUE ===
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
    //           ATAQUE
    // ===========================
    IEnumerator Attack()
    {
        isAttacking = true;

        // Trigger da animação
        if (animator != null)
            animator.SetTrigger(attackTrigger);

        // SOM DE ATAQUE
        if (audioSource != null && attackSound != null)
            audioSource.PlayOneShot(attackSound);

        ShowAttackPointSprite(true);

        Collider[] hitEnemies = Physics.OverlapSphere(
            attackPoint.position,
            attackRange,
            enemyLayers
        );

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out EnemyController ec))
                ec.TakeDamage(attackDamage);

            if (enemy.TryGetComponent(out BanditAI bandit))
                bandit.TakeDamage(attackDamage);

            if (enemy.TryGetComponent(out KnightAI knight))
                knight.TakeDamage(attackDamage);

            if (enemy.TryGetComponent(out WolfAI wolf))
                wolf.TakeDamage(attackDamage);

            if (enemy.TryGetComponent(out CerberusAI cerberus))
                cerberus.TakeDamage(attackDamage);

            if (enemy.TryGetComponent(out KnightCaptainAI knightcaptain))
                knightcaptain.TakeDamage(attackDamage);

            if (enemy.TryGetComponent(out DemonSlimeAI demonslime))
                demonslime.TakeDamage(attackDamage);

            if (enemy.TryGetComponent(out GuardsAI guards))
                guards.TakeDamage(attackDamage);
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
        if (attackPoint == null) return;

        SpriteRenderer sr = attackPoint.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = show;

        if (show)
            attackPointTimer = attackPointVisibleDuration;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
