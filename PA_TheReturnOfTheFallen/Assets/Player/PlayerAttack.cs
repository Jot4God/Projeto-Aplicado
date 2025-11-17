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
    public Animator animator; // arrasta o Animator do player aqui
    public string attackTrigger = "Attack"; // nome do trigger no Animator
    // ===========================

    // ===== SISTEMA DE ARMAS =====
    [System.Serializable]
    public class WeaponSlot
    {
        public string slotName = "Weapon Slot";
        public WeaponData weaponData;     // ScriptableObject com os dados da arma
        public GameObject weaponObject;   // filho com SpriteRenderer + Animator da arma
    }

    [Header("Armas")]
    public WeaponSlot[] weapons;          // lista de armas
    public int currentWeaponIndex = 0;    // arma ativa (0 = arma1, 1 = arma2, 2 = arma3)

    private Vector3 attackPointBaseLocalPos; // posição base do attackPoint

    [Header("Skilltree / Arma")]
    public PlayerWeaponAdapter weaponAdapter; // NOVO: adapter que aplica bónus do skilltree na arma
    // ============================

    void Start()
    {
        playerMana = GetComponent<PlayerMana>();

        // se não ligaste no inspector, tenta apanhar no próprio player ou filho
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (weaponAdapter == null)
            weaponAdapter = GetComponent<PlayerWeaponAdapter>();

        // guardar posição base do attackPoint para aplicar offsets das armas
        if (attackPoint != null)
            attackPointBaseLocalPos = attackPoint.localPosition;

        // desativar todas as armas no início (só aparecem quando atacas)
        HideAllWeapons();

        // aplicar stats da arma inicial (se existir)
        ApplyCurrentWeaponStats();
    }

    void Update()
    {
        // ---- TROCA DE ARMAS ----
        // Alpha1 -> arma 0, Alpha2 -> arma 1, Alpha3 -> arma 2
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(2);
        }
        // ------------------------

        // Ataque com botão esquerdo do rato
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            if (playerMana != null && playerMana.UseMana(manaCost))
            {
                StartCoroutine(Attack());
            }
            else
            {
                Debug.Log("❌ Sem mana suficiente para atacar!");
            }
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        // toca animação de ataque do PLAYER
        if (animator != null && !string.IsNullOrEmpty(attackTrigger))
            animator.SetTrigger(attackTrigger);

        // ativa visual da arma atual + animação da arma
        ShowCurrentWeapon();
        PlayCurrentWeaponAttackAnimation();

        // Dano
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            EnemyController ec = enemy.GetComponent<EnemyController>();
            if (ec != null) ec.TakeDamage(attackDamage);

            BanditAI bandit = enemy.GetComponent<BanditAI>();
            if (bandit != null) bandit.TakeDamage(attackDamage);

            KnightAI knight = enemy.GetComponent<KnightAI>();
            if (knight != null) knight.TakeDamage(attackDamage);

            WolfAI wolf = enemy.GetComponent<WolfAI>();
            if (wolf != null) wolf.TakeDamage(attackDamage);

            CerberusAI cerberus = enemy.GetComponent<CerberusAI>();
            if (cerberus != null) cerberus.TakeDamage(attackDamage);

            KnightCaptainAI knightcaptain = enemy.GetComponent<KnightCaptainAI>();
            if (knightcaptain != null) knightcaptain.TakeDamage(attackDamage);

            DemonSlimeAI demonslime = enemy.GetComponent<DemonSlimeAI>();
            if (demonslime != null) demonslime.TakeDamage(attackDamage);
        }

        // espera o cooldown da arma
        yield return new WaitForSeconds(attackCooldown);

        // ao acabar o ataque, esconder armas
        HideAllWeapons();
        isAttacking = false;
    }

    // =========================
    // FUNÇÕES AUXILIARES
    // =========================
    void SwitchWeapon(int index)
    {
        if (weapons == null || weapons.Length == 0) return;
        if (index < 0 || index >= weapons.Length) return;
        if (currentWeaponIndex == index) return;

        currentWeaponIndex = index;
        ApplyCurrentWeaponStats();
        Debug.Log("🔁 Arma trocada para: " + GetCurrentWeaponName());
    }

    void ApplyCurrentWeaponStats()
    {
        if (weapons == null || weapons.Length == 0) return;

        WeaponSlot slot = weapons[currentWeaponIndex];
        if (slot == null || slot.weaponData == null) return;

        WeaponData data = slot.weaponData;

        // aplicar stats da arma
        attackDamage = data.attackDamage;
        attackCooldown = data.attackCooldown;
        attackRange = data.attackRange;

        // atualizar posição do attackPoint
        if (attackPoint != null)
            attackPoint.localPosition = attackPointBaseLocalPos + data.attackPointLocalOffset;

        // atualizar visual da arma atual (sprite + animator)
        if (slot.weaponObject != null)
        {
            SpriteRenderer sr = slot.weaponObject.GetComponent<SpriteRenderer>();
            if (sr != null && data.weaponSprite != null)
                sr.sprite = data.weaponSprite;

            Animator weaponAnim = slot.weaponObject.GetComponent<Animator>();
            if (weaponAnim != null && data.weaponAnimator != null)
                weaponAnim.runtimeAnimatorController = data.weaponAnimator;

            // manter desativado até ataque
            slot.weaponObject.SetActive(false);
        }

        // >>> NOVO: informar o WeaponAdapter da base da arma (sem skilltree)
        if (weaponAdapter != null)
        {
            weaponAdapter.SetBaseWeaponStats(attackDamage, attackCooldown);
        }
    }

    string GetCurrentWeaponName()
    {
        if (weapons == null || weapons.Length == 0) return "No Weapon";
        WeaponSlot slot = weapons[currentWeaponIndex];
        if (slot != null && slot.weaponData != null) return slot.weaponData.weaponName;
        return "Unnamed Weapon";
    }

    void HideAllWeapons()
    {
        if (weapons == null) return;
        foreach (var slot in weapons)
        {
            if (slot != null && slot.weaponObject != null)
                slot.weaponObject.SetActive(false);
        }
    }

    void ShowCurrentWeapon()
    {
        if (weapons == null || weapons.Length == 0) return;
        WeaponSlot slot = weapons[currentWeaponIndex];
        if (slot != null && slot.weaponObject != null)
            slot.weaponObject.SetActive(true);
    }

    void PlayCurrentWeaponAttackAnimation()
    {
        if (weapons == null || weapons.Length == 0) return;

        WeaponSlot slot = weapons[currentWeaponIndex];
        if (slot == null || slot.weaponObject == null || slot.weaponData == null) return;

        Animator weaponAnim = slot.weaponObject.GetComponent<Animator>();
        if (weaponAnim == null) return;

        string triggerName = slot.weaponData.weaponAttackTrigger;
        if (!string.IsNullOrEmpty(triggerName))
        {
            weaponAnim.ResetTrigger(triggerName); // só para garantir que limpa
            weaponAnim.SetTrigger(triggerName);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
