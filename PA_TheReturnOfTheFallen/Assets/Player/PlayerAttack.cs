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

    // ===== SOM: SEPARADO (MISS/SWING vs HIT) =====
    [Header("Áudio (Separado)")]
    [Tooltip("AudioSource para o som do ataque quando NÃO acerta (swing/miss).")]
    public AudioSource attackAudioSource;

    [Tooltip("AudioSource para o som quando ACERTA (hit/impact).")]
    public AudioSource hitAudioSource;

    [Header("Clips")]
    [Tooltip("Som do swing/miss (toca apenas se NÃO acertar).")]
    public AudioClip attackSound;

    [Tooltip("Som do impacto/hit (toca apenas se acertar).")]
    public AudioClip hitSound;

    [Tooltip("Se true, toca o hit apenas 1x por ataque, mesmo acertando em vários inimigos.")]
    public bool playHitSoundOncePerAttack = true;

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

        // Se não preencheres no Inspector, tenta apanhar automaticamente
        if (attackAudioSource == null)
            attackAudioSource = GetComponent<AudioSource>();

        // hitAudioSource fica por preencher se quiseres um separado;
        // recomenda-se criar um segundo AudioSource no Player (ou child) e arrastar no Inspector.
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

        // Trigger da animação (sempre)
        if (animator != null)
            animator.SetTrigger(attackTrigger);

        ShowAttackPointSprite(true);

        // 1) Ver quem foi atingido primeiro
        Collider[] hitEnemies = Physics.OverlapSphere(
            attackPoint.position,
            attackRange,
            enemyLayers
        );

        bool hitSomething = false;
        bool hitSoundPlayed = false;

        foreach (Collider enemy in hitEnemies)
        {
            bool didDamageThisCollider = false;

            if (enemy.TryGetComponent(out EnemyController ec))
            {
                ec.TakeDamage(attackDamage);
                didDamageThisCollider = true;
            }

            if (enemy.TryGetComponent(out BanditAI bandit))
            {
                bandit.TakeDamage(attackDamage);
                didDamageThisCollider = true;
            }

            if (enemy.TryGetComponent(out KnightAI knight))
            {
                knight.TakeDamage(attackDamage);
                didDamageThisCollider = true;
            }

            if (enemy.TryGetComponent(out WolfAI wolf))
            {
                wolf.TakeDamage(attackDamage);
                didDamageThisCollider = true;
            }

            if (enemy.TryGetComponent(out CerberusAI cerberus))
            {
                cerberus.TakeDamage(attackDamage);
                didDamageThisCollider = true;
            }

            if (enemy.TryGetComponent(out KnightCaptainAI knightcaptain))
            {
                knightcaptain.TakeDamage(attackDamage);
                didDamageThisCollider = true;
            }

            if (enemy.TryGetComponent(out DemonSlimeAI demonslime))
            {
                demonslime.TakeDamage(attackDamage);
                didDamageThisCollider = true;
            }

            if (enemy.TryGetComponent(out GuardsAI guards))
            {
                guards.TakeDamage(attackDamage);
                didDamageThisCollider = true;
            }

            if (didDamageThisCollider)
            {
                hitSomething = true;

                // Se quiseres tocar hit por cada inimigo (não recomendado), mete playHitSoundOncePerAttack = false
                if (!playHitSoundOncePerAttack)
                {
                    PlayHitSound();
                }
                else if (!hitSoundPlayed)
                {
                    PlayHitSound();
                    hitSoundPlayed = true;
                }
            }
        }

        // 2) Garantir que NÃO mistura: ou hit OU swing/miss
        if (!hitSomething)
        {
            PlayAttackMissSound();
        }
        else
        {
            // Se for "once per attack" e ainda não tocou (caso raro), toca aqui
            if (playHitSoundOncePerAttack && !hitSoundPlayed)
                PlayHitSound();
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void PlayAttackMissSound()
    {
        if (attackAudioSource != null && attackSound != null)
            attackAudioSource.PlayOneShot(attackSound);
    }

    void PlayHitSound()
    {
        if (hitAudioSource != null && hitSound != null)
        {
            hitAudioSource.PlayOneShot(hitSound);
            return;
        }

        // Fallback: se não tiveres hitAudioSource atribuído, usa o attackAudioSource
        if (attackAudioSource != null && hitSound != null)
            attackAudioSource.PlayOneShot(hitSound);
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
