using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BanditAI : MonoBehaviour
{
    [Header("Movimento")]
    public Transform player;
    public float speed = 4f;
    public float chaseDistance = 8f; // Distância de detecção para perseguir o jogador
    public float attackRange = 1.5f; // Range onde o inimigo aplica o dano
    public Transform[] patrolPoints;
    public float patrolWait = 2f;

    [Header("Vida")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Ataque")]
    public int damage = 30;            // 🔹 Dano que este inimigo causa
    public float attackCooldown = 1f;  // Tempo entre ataques
    private float lastAttackTime = 0f; // Controle do tempo de cooldown

    [Header("Recompensas")]
    public int xpReward = 20;
    public GameObject healthPickupPrefab;
    public GameObject moneyPickupPrefab;
    public int moneyDropAmount = 1;

    [Header("Animation")]
    public Animator animator;                 // <-- para controlar Idle/Run
    public string runningBool = "isRunning";  // <-- nome do bool no Animator
    public string attackTrigger = "Attack";  // <-- trigger para animação de ataque

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

        // Inicializa o SpriteRenderer para alteração de cor
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // Guarda a cor original
        }

        // Inicia com o flip para a direita (de frente)
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = false; // Começa virado para a direita (sem flip)
        }
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Verifica se o jogador está dentro do range de detecção para perseguir ou atacar
        if (dist <= attackRange) state = State.Attacking;  // Dentro do range de ataque, pode atacar
        else if (dist <= chaseDistance) state = State.Chasing; // Se estiver dentro do range de perseguição, começa a perseguir
        else state = State.Patrolling; // Se não estiver dentro de nenhum dos ranges, patrulha

        // Realiza ações de acordo com o estado
        switch (state)
        {
            case State.Patrolling: Patrol(); break;
            case State.Chasing: Chase(); break;
            case State.Attacking: Attack(); break;
        }

        // ===== animação =====
        // Atualiza animação de corrida se estiver perseguindo o jogador
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
            if (currentDirection.x < 0)
            {
                // Se o inimigo se move para a esquerda, faz o flip
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = false; // Fica de costas (flip)
                }
            }
            else if (currentDirection.x > 0)
            {
                // Se o inimigo se move para a direita, desfaz o flip
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = true; // Fica de frente (sem flip)
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
        // Só realiza o ataque se o cooldown for cumprido
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            // Chama a animação de ataque
            if (animator != null)
                animator.SetTrigger(attackTrigger);

            // Aplica o dano se o jogador está dentro do range de ataque
            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                PlayerHP ph = player.GetComponent<PlayerHP>();
                if (ph != null)
                {
                    ph.TakeDamage(damage); // Aplica o dano ao jogador
                    lastAttackTime = Time.time; // Atualiza o tempo de cooldown
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log(name + " recebeu " + damage + " de dano! Vida atual: " + currentHealth);

        // Muda a cor para vermelho ao levar dano
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            // Restaura a cor original após um curto tempo
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
        Gizmos.DrawWireSphere(transform.position, attackRange);  // Range de ataque
    }
}
