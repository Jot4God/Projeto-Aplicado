using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public Transform attackPoint;      // Ponta da espada
    public float attackRange = 1.5f;   // Alcance do ataque
    public LayerMask enemyLayers;      // Layer dos inimigos
    public int attackDamage = 20;      // Dano do ataque
    public GameObject sword;           // A espada já na mão
    public float attackCooldown = 0.3f;

    public int manaCost = 10;          // Custo de mana por ataque
    private PlayerMana playerMana;     // Referência ao sistema de mana
    private bool isAttacking = false;

    void Start()
    {
        playerMana = GetComponent<PlayerMana>();
    }

    void Update()
{
    // Mouse1 = 0, Mouse2 = 1
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

        // 1️⃣ Dano aos inimigos
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            EnemyController ec = enemy.GetComponent<EnemyController>();
            if (ec != null)
                ec.TakeDamage(attackDamage);
        }

        // 2️⃣ Animação visual simples da espada
        if (sword != null)
        {
            float swingAngle = 90f;
            float swingTime = attackCooldown;
            float elapsed = 0f;

            while (elapsed < swingTime)
            {
                float step = (swingAngle / swingTime) * Time.deltaTime;
                sword.transform.Rotate(Vector3.forward * step);
                elapsed += Time.deltaTime;
                yield return null;
            }

            sword.transform.Rotate(Vector3.forward * -swingAngle);
        }

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
