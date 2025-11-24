using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KnightCaptainAI : MonoBehaviour
{
    [Header("Movimento")]
    public Transform player;
    public float speed = 4f;
    public float chaseDistance = 8f;
    public float attackRange = 1.5f;
    public Transform[] patrolPoints;
    public float patrolWait = 2f;

    [Header("Vida")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Ataque")]
    public int damage = 30;
    public float attackCooldown = 1f;
    private float lastAttackTime = 0f;

    [Header("Hitbox de Ataque")]
    public Transform attackPoint;
    public float attackRadius = 0.8f;
    public LayerMask playerLayer;

    private bool canApplyDamage = false;
    private bool hasAppliedDamageThisSwing = false;

    [Header("Recompensas")]
    public int xpReward = 20;
    public GameObject healthPickupPrefab;
    public GameObject moneyPickupPrefab;
    public int moneyDropAmount = 1;

    [Header("Animation")]
    public Animator animator;
    public string runningBool = "isRunning";
    public string attackTrigger = "Attack";

    private int currentPatrol = 0;
    private float waitTimer = 0f;
    private Rigidbody rb;
    private Vector3 currentDirection = Vector3.zero;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private enum State { Patrolling, Chasing, Attacking }
    private State state = State.Patrolling;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = true;

        // ⛔ impede o inimigo de mexer no Y
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

        currentHealth = maxHealth;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            spriteRenderer.flipX = false;
        }
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange) state = State.Attacking;
        else if (dist <= chaseDistance) state = State.Chasing;
        else state = State.Patrolling;

        switch (state)
        {
            case State.Patrolling:
                Patrol();
                break;
            case State.Chasing:
                Chase();
                break;
            case State.Attacking:
                Attack();
                break;
        }

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

            if (spriteRenderer != null)
            {
                if (currentDirection.x < 0f)
                    spriteRenderer.flipX = true;
                else if (currentDirection.x > 0f)
                    spriteRenderer.flipX = false;
            }
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
        Vector3 dir = target - transform.position;

        // ⛔ impede movimento vertical
        dir.y = 0f;

        currentDirection = dir.normalized;

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
        Vector3 dir = player.position - transform.position;

        // ⛔ impedir inimigo de subir/descer
        dir.y = 0f;

        currentDirection = dir.normalized;
    }

    void Attack()
    {
        currentDirection = Vector3.zero;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (animator != null)
                animator.SetTrigger(attackTrigger);

            canApplyDamage = true;
            hasAppliedDamageThisSwing = false;

            lastAttackTime = Time.time;
        }
    }

    public void OnAttackHit()
    {
        if (!canApplyDamage || hasAppliedDamageThisSwing) return;
        if (attackPoint == null)
        {
            Debug.LogWarning("AttackPoint não está definido no KnightCaptainAI.");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRadius, playerLayer);

        foreach (Collider hit in hits)
        {
            PlayerHP ph = hit.GetComponent<PlayerHP>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
                hasAppliedDamageThisSwing = true;
                break;
            }
        }
    }

    public void OnAttackEnd()
    {
        canApplyDamage = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log(name + " recebeu " + damage + " de dano! Vida atual: " + currentHealth);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            Invoke("RestoreColor", 0.1f);
        }

        if (currentHealth <= 0)
            Die();
    }

    void RestoreColor()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
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
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (attackPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
