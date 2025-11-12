using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public Transform attackPoint;      
    public float attackRange = 1.5f;   
    public LayerMask enemyLayers;      
    public int attackDamage = 20;      
    public float attackCooldown = 0.3f;

    public int manaCost = 10;          
    private PlayerMana playerMana;     
    private bool isAttacking = false;

    // Sprite do ponto de ataque
    public GameObject attackSprite;    
    public float spriteTime = 0.15f;   

    // ===== ADICIONADO PARA ANIMAÇÃO =====
    public Animator animator;                  // arrasta o Animator do player aqui
    public string attackTrigger = "Attack";    // nome do trigger no Animator
    // =====================================

    void Start()
    {
        playerMana = GetComponent<PlayerMana>();
        if (attackSprite != null)
            attackSprite.SetActive(false); // começa invisível

        // se não ligaste no inspector, tenta apanhar no próprio player ou filho
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
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

        // toca animação de ataque
        if (animator != null && !string.IsNullOrEmpty(attackTrigger))
            animator.SetTrigger(attackTrigger);

        // Dano
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            EnemyController ec = enemy.GetComponent<EnemyController>();
            if (ec != null)
                ec.TakeDamage(attackDamage);

            // se tiveres outros tipos de inimigos:
            BanditAI bandit = enemy.GetComponent<BanditAI>();
            if (bandit != null)
                bandit.TakeDamage(attackDamage);
            KnightAI knight = enemy.GetComponent<KnightAI>();
            if (knight != null)
                bandit.TakeDamage(attackDamage);
        }

        // Mostrar sprite do ataque
        if (attackSprite != null)
        {
            attackSprite.SetActive(true);
            yield return new WaitForSeconds(spriteTime);
            attackSprite.SetActive(false);
        }

        // cooldown
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
