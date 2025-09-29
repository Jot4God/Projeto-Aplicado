using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float speed = 4f;
    public float chaseDistance = 8f;
    public float attackDistance = 1.5f;
    public Transform[] patrolPoints;
    public float patrolWait = 2f;

    // Vida do inimigo
    public int maxHealth = 50;
    private int currentHealth;

    private int currentPatrol = 0;
    private float waitTimer = 0f;
    private Rigidbody rb;
    private enum State { Patrolling, Chasing, Attacking }
    private State state = State.Patrolling;

    private float attackCooldown = 1f;
    private float lastAttackTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        currentHealth = maxHealth; // Inicializa vida

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackDistance) state = State.Attacking;
        else if (dist <= chaseDistance) state = State.Chasing;
        else state = State.Patrolling;

        switch (state)
        {
            case State.Patrolling: Patrol(); break;
            case State.Chasing: Chase(); break;
            case State.Attacking: Attack(); break;
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) 
        { 
            rb.linearVelocity = Vector3.zero; 
            return; 
        }

        Vector3 target = patrolPoints[currentPatrol].position;
        Vector3 moveDir = (target - transform.position).normalized;

        rb.linearVelocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);

        if (Vector3.Distance(transform.position, target) < 0.3f)
        {
            rb.linearVelocity = Vector3.zero;
            waitTimer += Time.deltaTime;
            if (waitTimer >= patrolWait)
            {
                currentPatrol = (currentPatrol + 1) % patrolPoints.Length;
                waitTimer = 0f;
            }
        }
    }

    void Chase()
    {
        Vector3 moveDir = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);
    }

    void Attack()
    {
        rb.linearVelocity = Vector3.zero; 

        if (player == null) return;

        PlayerHP ph = player.GetComponent<PlayerHP>();
        if (ph != null && Time.time >= lastAttackTime + attackCooldown)
        {
            ph.TakeDamage(10);
            lastAttackTime = Time.time;
        }
    }

    // **Novo método para receber dano**
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log(name + " recebeu " + damage + " de dano! Vida atual: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(name + " morreu!");
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
