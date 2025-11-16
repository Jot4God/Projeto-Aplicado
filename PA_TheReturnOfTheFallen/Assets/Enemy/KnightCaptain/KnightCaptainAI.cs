using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KnightCaptainAI : MonoBehaviour
{
    [Header("Movimento")]
    public Transform player;
    public float speed = 4f;
    public float chaseDistance = 8f; // Distância de detecção para perseguir o jogador
    public float attackRange = 1.5f; // Distância para COMEÇAR a animação de ataque (dano é pela hitbox)
    public Transform[] patrolPoints;
    public float patrolWait = 2f;

    [Header("Vida")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Ataque")]
    public int damage = 30;            // Dano que este inimigo causa
    public float attackCooldown = 1f;  // Tempo entre ataques
    private float lastAttackTime = 0f; // Controle do tempo de cooldown

    [Header("Hitbox de Ataque")]
    public Transform attackPoint;      // Ponto à frente do boss onde a hitbox vai ser verificada
    public float attackRadius = 0.8f;  // Raio da hitbox
    public LayerMask playerLayer;      // Layer do Player

    private bool canApplyDamage = false;          // Se este ataque pode aplicar dano
    private bool hasAppliedDamageThisSwing = false; // Para não dar dano múltiplas vezes no mesmo ataque

    [Header("Recompensas")]
    public int xpReward = 20;
    public GameObject healthPickupPrefab;
    public GameObject moneyPickupPrefab;
    public int moneyDropAmount = 1;

    [Header("Animation")]
    public Animator animator;                 // <-- para controlar Idle/Run
    public string runningBool = "isRunning";  // <-- nome do bool no Animator
    public string attackTrigger = "Attack";   // <-- trigger para animação de ataque

    private int currentPatrol = 0;
    private float waitTimer = 0f;
    private Rigidbody rb;
    private Vector3 currentDirection = Vector3.zero;

    private SpriteRenderer spriteRenderer;   // Para alterar a cor do inimigo
    private Color originalColor;             // Cor original do inimigo

    private enum State { Patrolling, Chasing, Attacking }
    private State state = State.Patrolling;

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

        // Apanha o Animator automaticamente se não estiver ligado
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // Inicializa o SpriteRenderer (geralmente está no child)
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // Guarda a cor original
            spriteRenderer.flipX = false;        // Começa virado para a direita (sem flip)
        }
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Decide estado apenas com base nas distâncias (sem aplicar dano aqui!)
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

        // Animação de corrida só quando está a perseguir
        if (animator != null)
        {
            bool shouldRun = (state == State.Chasing);
            animator.SetBool(runningBool, shouldRun);
        }

        // Debug para aplicar dano manualmente
        if (Input.GetKeyDown(KeyCode.K))
            TakeDamage(10);
    }

    void FixedUpdate()
    {
        if (currentDirection != Vector3.zero)
        {
            Vector3 newPos = transform.position + currentDirection * speed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);

            // Flip horizontal depende da direção do movimento
            if (spriteRenderer != null)
            {
                if (currentDirection.x < 0f)
                {
                    // Vai para a esquerda → flip
                    spriteRenderer.flipX = true;
                }
                else if (currentDirection.x > 0f)
                {
                    // Vai para a direita → sem flip
                    spriteRenderer.flipX = false;
                }
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

    // ====== ESTES MÉTODOS SÃO CHAMADOS PELA ANIMAÇÃO ======

    // Chama este método num Animation Event no frame em que a espada/arma devia “acertar”
    public void OnAttackHit()
    {
        if (!canApplyDamage || hasAppliedDamageThisSwing) return;
        if (attackPoint == null)
        {
            Debug.LogWarning("AttackPoint não está definido no KnightCaptainAI.");
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

    // =======================================

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log(name + " recebeu " + damage + " de dano! Vida atual: " + currentHealth);

        // Muda a cor para vermelho ao levar dano
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
        {
            spriteRenderer.color = originalColor;
        }
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
        // Gizmos de Debug para o range de detecção e o range de ataque
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance); // Range de perseguição

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);  // Distância para começar ataque

        // Gizmo da hitbox de ataque
        if (attackPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
