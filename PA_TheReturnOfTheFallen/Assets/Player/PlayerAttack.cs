using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform attackPoint;      
    public float attackRange = 1f;    
    public LayerMask enemyLayers;    
    public int attackDamage = 20;      
    public float slashDuration = 0.3f; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Attack();
    }

    void Attack()
    {
        
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            EnemyController ec = enemy.GetComponent<EnemyController>();
            if (ec != null)
                ec.TakeDamage(attackDamage);
        }

        GameObject slash = GameObject.CreatePrimitive(PrimitiveType.Cube);
        slash.transform.position = attackPoint.position;
        slash.transform.localScale = new Vector3(0.1f, 0.1f, attackRange * 2);
        Destroy(slash.GetComponent<Collider>());

        Renderer rend = slash.GetComponent<Renderer>();
        if (rend != null) rend.material.color = Color.red;

        
        slash.transform.Rotate(Vector3.up * 45); 
        StartCoroutine(RotateSlash(slash));

        Destroy(slash, slashDuration);
    }

    System.Collections.IEnumerator RotateSlash(GameObject slash)
    {
        float elapsed = 0f;
        while (elapsed < slashDuration)
        {
            slash.transform.Rotate(Vector3.up * 720 * Time.deltaTime); 
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
