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

    // Prefab do health pickup
    public GameObject healthPickupPrefab;
    public GameObject moneyPickupPrefab; // Prefab da moeda
    public int moneyDropAmount = 1;      // Quantas moedas dropar

    private int currentPatrol = 0;
    private float waitTimer = 0f;
    private Rigidbody rb;

    private enum State { Patrolling, Chasing, Attacking }
    private State state = State.Patrolling;

    private float attackCooldown = 1f;
    private float lastAttackTime = 0f;

    private Vector3 currentDirection = Vector3.zero; // <- guarda direção para mover no FixedUpdate

    void Awake()
    {
        // 🔹 Se este inimigo é o "prefab base" na cena (não instanciado em runtime),
        // desativa-o apenas uma vez no início.
        if (!Application.isEditor && gameObject.scene.rootCount > 0 && transform.parent == null && !gameObject.name.Contains("(Clone)"))
        {
            gameObject.SetActive(false);
            return;
        }

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

    void Patrol()
    {
        if (patrolPoints.Length == 0)
        {
            currentDirection = Vector3.zero;
            return;
        }

        Vector3 target = patrolPoints[currentPatrol].position;
        Vector3 moveDir = (target - transform.position).normalized;
        currentDirection = moveDir;

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
        Vector3 moveDir = (player.position - transform.position).normalized;
        currentDirection = moveDir;
    }

    void Attack()
    {
        currentDirection = Vector3.zero; // pára de mover

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

    // Chance de dropar vida (ex: 50%)
    if (healthPickupPrefab != null && Random.value < 0.5f)
    {
        Vector3 spawnPos = transform.position + new Vector3(0f, 0.5f, 0f);
        Instantiate(healthPickupPrefab, spawnPos, Quaternion.identity);
    }

    // Chance de dropar moedas (ex: 70%)
    if (moneyPickupPrefab != null && Random.value < 0.7f)
    {
        float radius = 1f; // distância do inimigo
        for (int i = 0; i < moneyDropAmount; i++)
        {
            // spawn aleatório em volta do inimigo
            Vector3 spawnPos = transform.position + new Vector3(
                Random.Range(-radius, radius), 
                0.5f, 
                Random.Range(-radius, radius)
            );
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
