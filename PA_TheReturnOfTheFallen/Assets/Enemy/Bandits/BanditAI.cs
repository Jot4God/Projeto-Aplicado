using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BanditAI : MonoBehaviour
{
    [Header("Movimento")]
    public Transform player;
    public float speed = 4f;
    public float chaseDistance = 8f;
    public float attackDistance = 1.5f;
    public Transform[] patrolPoints;
    public float patrolWait = 2f;

    [Header("Vida")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Ataque")]
    public int damage = 30;            // 🔹 Dano que este inimigo causa
    public float attackCooldown = 1f;  // Tempo entre ataques

    [Header("Recompensas")]
    public int xpReward = 20;
    public GameObject healthPickupPrefab;
    public GameObject moneyPickupPrefab;
    public int moneyDropAmount = 1;

    [Header("Animation")]
    public Animator animator;                 // <-- para controlar Idle/Run
    public string runningBool = "isRunning";  // <-- nome do bool no Animator

    private int currentPatrol = 0;
    private float waitTimer = 0f;
    private Rigidbody rb;
    private Vector3 currentDirection = Vector3.zero;

    private enum State { Patrolling, Chasing, Attacking }
    private State state = State.Patrolling;

    private float lastAttackTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        currentHealth = maxHealth;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        // apanha o Animator automaticamente se não estiver ligado
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
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

        // ===== animação =====
        // só corre quando está em CHASING
        if (animator != null)
        {
            bool shouldRun = (state == State.Chasing);
            animator.SetBool(runningBool, shouldRun);
        }

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

    void Patrol()
    {
        if (patrolPoints.Length == 0)
        {
            currentDirection = Vector3.zero;
            return;
        }

        Vector3 target = patrolPoints[currentPatrol].position;
        currentDirection = (target - transform.position).normalized;

        if (Vector3.Distance(transform.position, target) < 0.3f)
        {
            currentDirection = Vector3.zero;
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
        currentDirection = (player.position - transform.position).normalized;
    }
    
void Attack()
{
    currentDirection = Vector3.zero;

    if (Time.time >= lastAttackTime + attackCooldown)
    {
        // toca a animação de ataque
        if (animator != null)
            animator.SetTrigger("Attack");

        PlayerHP ph = player.GetComponent<PlayerHP>();
        if (ph != null)
        {
            ph.TakeDamage(damage);
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

        if (player != null)
        {
            PlayerLevel playerLevel = player.GetComponent<PlayerLevel>();
            if (playerLevel != null)
            {
                playerLevel.AddXP(xpReward);
                Debug.Log("Jogador ganhou " + xpReward + " XP!");
            }
        }

        if (healthPickupPrefab != null && Random.value < 0.5f)
        {
            Vector3 spawnPos = transform.position + new Vector3(0f, -5f, 0f);
            Instantiate(healthPickupPrefab, spawnPos, Quaternion.identity);
        }

        if (moneyPickupPrefab != null && Random.value < 0.7f)
        {
            for (int i = 0; i < moneyDropAmount; i++)
            {
                Vector3 spawnPos = transform.position + new Vector3(0f, -5f, 0f);
                Instantiate(moneyPickupPrefab, spawnPos, Quaternion.identity);
            }
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
