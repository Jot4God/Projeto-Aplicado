using UnityEngine;

public class SpellHit : MonoBehaviour
{
    public int damage = 20;
    public LayerMask enemyLayers;

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyLayers) != 0)
        {
            // Inimigo genérico
            EnemyController ec = other.GetComponent<EnemyController>();
            if (ec != null)
                ec.TakeDamage(damage);

            // Bandit
            BanditAI bandit = other.GetComponent<BanditAI>();
            if (bandit != null)
                bandit.TakeDamage(damage);

            // Knight
            KnightAI knight = other.GetComponent<KnightAI>();
            if (knight != null)
                knight.TakeDamage(damage);

            // Wolf
            WolfAI wolf = other.GetComponent<WolfAI>();
            if (wolf != null)
                wolf.TakeDamage(damage);

            // Cerberus
            CerberusAI cerberus = other.GetComponent<CerberusAI>();
            if (cerberus != null)
                cerberus.TakeDamage(damage);

            DemonSlimeAI demonslime = other.GetComponent<DemonSlimeAI>();
            if (demonslime != null)
                demonslime.TakeDamage(damage);

            GuardsAI guards = other.GetComponent<GuardsAI>();
            if (guards != null)
                guards.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
