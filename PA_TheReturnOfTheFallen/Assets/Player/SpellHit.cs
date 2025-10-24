using UnityEngine;

public class SpellHit : MonoBehaviour
{
    public int damage = 20;
    public LayerMask enemyLayers;

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyLayers) != 0)
        {
            EnemyController ec = other.GetComponent<EnemyController>();
            if (ec != null)
                ec.TakeDamage(damage);
            
            Destroy(gameObject); 
        }
    }
}
