using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class DemonSlimeAI : MonoBehaviour
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
    public Transform attackPoint;      // Ponto onde a hitbox é verificada (frente do slime)
    public float attackRadius = 0.8f;  // Raio da hitbox
    public LayerMask playerLayer;      // Layer do Player

    // flags para controlo do dano por animação
    private bool canApplyDamage = false;             // se este ataque pode aplicar dano
    private bool hasAppliedDamageThisSwing = false;  // para não dar dano múltiplas vezes no mesmo ataque

    [Header("Recompensas")]
    public int xpReward = 20;
    public GameObject healthPickupPrefab;
    public GameObject moneyPickupPrefab;
    public int moneyDropAmount = 1;

    [Header("Animation")]
    public Animator animator;                 
    public string runningBool = "isRunning";  
    public string attackTrigger = "Attack";   

    [Header("Projéteis de Fogo")]
    public GameObject fireballPrefab;
    public float fireballSpeed = 10f;
    public float fireballCooldown = 5f;
    private float lastFireballTime = 0f;

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
        rb.useGravity = false;

        currentHealth = maxHealth;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (spriteRenderer != null)
            spriteRenderer.flipX = true;
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
            case State.Patrolling: Patrol(); break;
            case State.Chasing:   Chase();  break;
            case State.Attacking: Attack(); break;
        }

        if (animator != null)
        {
            bool shouldRun = (state == State.Chasing);
            animator.SetBool(runningBool, shouldRun);
        }

        // FIREBALL UPDATE CORRIGIDO
        if (Time.time >= lastFireballTime + fireballCooldown && state == State.Chasing)
            ShootFireball();

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
                if (currentDirection.x < 0) spriteRenderer.flipX = false;
                else if (currentDirection.x > 0) spriteRenderer.flipX = true;
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
        // Quando está a atacar, paramos o movimento
        currentDirection = Vector3.zero;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (animator != null)
                animator.SetTrigger(attackTrigger);

            // Marca que este ataque pode dar dano (quando o evento de animação disparar)
            canApplyDamage = true;
            hasAppliedDamageThisSwing = false;

            lastAttackTime = Time.time;
        }
    }

    // ===== ESTES MÉTODOS SÃO CHAMADOS PELA ANIMAÇÃO (HITBOX) =====

    // Chama este método num Animation Event no frame em que o slime devia “acertar”
    public void OnAttackHit()
    {
        if (!canApplyDamage || hasAppliedDamageThisSwing) return;
        if (attackPoint == null)
        {
            Debug.LogWarning("AttackPoint não está definido no DemonSlimeAI.");
            return;
        }

        // Procura o Player dentro da hitbox
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRadius, playerLayer);

        foreach (Collider hit in hits)
        {
            PlayerHP ph = hit.GetComponent<PlayerHP>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
                hasAppliedDamageThisSwing = true; // Garante que só bate uma vez por swing
                break;
            }
        }
    }

    // Chama este método num Animation Event no fim da animação de ataque
    public void OnAttackEnd()
    {
        canApplyDamage = false;
    }

    // ===== FIREBALL 2D CORRIGIDO =====
    void ShootFireball()
    {
        if (fireballPrefab != null && player != null)
        {
            Vector3 dir = (player.position - transform.position).normalized;

            GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);

            // Aqui está a correção: a fireball usa Rigidbody2D
            Rigidbody2D rb2d = fireball.GetComponent<Rigidbody2D>();

            if (rb2d != null)
            {
                rb2d.linearVelocity = dir * fireballSpeed;
            }

            lastFireballTime = Time.time;
        }
    }
    // ===== FIM DA CORREÇÃO =====

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

    // 🔵 CARREGAR A CENA DE VITÓRIA
    SceneManager.LoadScene("GameVictory");

    Destroy(gameObject);
}

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Gizmo da hitbox de ataque (para veres onde está o ataque)
        if (attackPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
