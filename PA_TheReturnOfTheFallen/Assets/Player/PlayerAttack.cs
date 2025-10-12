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

    private bool isAttacking = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
        {
            StartCoroutine(Attack());
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
            float swingAngle = 90f; // swing horizontal
            float swingTime = attackCooldown;
            float elapsed = 0f;

            while (elapsed < swingTime)
            {
                float step = (swingAngle / swingTime) * Time.deltaTime;
                sword.transform.Rotate(Vector3.forward * step);
                elapsed += Time.deltaTime;
                yield return null;
            }

            sword.transform.Rotate(Vector3.forward * -swingAngle); // volta para posição inicial
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
