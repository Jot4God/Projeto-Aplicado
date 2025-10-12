using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float speed = 4f;
    public float chaseDistance = 8f;
    public float attackDistance = 1.5f;

    public int maxHealth = 50;
    private int currentHealth;

    public GameObject healthPickupPrefab;

    private Rigidbody rb;

    private enum State { Idle, Chasing, Attacking }
    private State state = State.Idle;

    private float attackCooldown = 1f;
    private float lastAttackTime = 0f;

    private Vector3 currentDirection = Vector3.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // impede empurrões
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        currentHealth = maxHealth;

        // Procura automaticamente o player pela tag
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackDistance)
            state = State.Attacking;
        else if (dist <= chaseDistance)
            state = State.Chasing;
        else
            state = State.Idle;

        switch (state)
        {
            case State.Idle:       FollowPlayer(); break; // em vez de patrulhar, segue o player sempre
            case State.Chasing:    FollowPlayer(); break;
            case State.Attacking:  Attack();       break;
        }

        // Teste manual de dano
        if (Input.GetKeyDown(KeyCode.K))
            TakeDamage(10);
    }

    void FixedUpdate()
    {
        if (currentDirection != Vector3.zero)
        {
            Vector3 newPos = transform.position + currentDirection * speed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);

        }
    }

    void FollowPlayer()
    {
        if (!player)
        {
            currentDirection = Vector3.zero;
            return;
        }

        Vector3 moveDir = (player.position - transform.position).normalized;
        currentDirection = moveDir;
    }

    void Attack()
    {
        currentDirection = Vector3.zero;
        if (player == null) return;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            PlayerHP ph = player.GetComponent<PlayerHP>();
            if (ph != null)
            {
                ph.TakeDamage(30);
                lastAttackTime = Time.time;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log(name + " recebeu " + damage + " de dano! Vida atual: " + currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log(name + " morreu!");

        if (healthPickupPrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0f, -5f, 0f);
            Instantiate(healthPickupPrefab, spawnPosition, Quaternion.identity);
        }

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
