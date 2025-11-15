using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpellMovement : MonoBehaviour
{
    public float speed = 40f;           // Velocidade do spell
    public float followRange = 10f;     // Distância máxima para seguir inimigos

    private Vector3 moveDirection = Vector3.right; // Direção inicial
    private Transform target;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Start()
    {
        // Aplica a direção inicial
        rb.linearVelocity = moveDirection * speed;
    }

    void FixedUpdate()
    {
        // Verifica inimigos próximos
        FindTarget();

        if (target != null)
        {
            Vector3 directionToTarget = target.position - transform.position;
            directionToTarget.y = 0;
            if (directionToTarget.magnitude <= followRange)
            {
                moveDirection = directionToTarget.normalized;
            }
        }

        // Aplica movimento contínuo via Rigidbody (Unity 2025+)
        rb.linearVelocity = moveDirection * speed;
    }

    public void SetDirection(Vector3 dir)
    {
        dir.y = 0;
        moveDirection = dir.normalized;

        if (rb != null)
            rb.linearVelocity = moveDirection * speed;
    }

    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            target = null;
            return;
        }

        float minDist = Mathf.Infinity;
        Transform closest = null;
        Vector3 pos = transform.position;

        foreach (GameObject e in enemies)
        {
            if (!e.activeInHierarchy) continue;

            float dist = Vector3.Distance(pos, e.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = e.transform;
            }
        }

        target = closest;
    }
}
