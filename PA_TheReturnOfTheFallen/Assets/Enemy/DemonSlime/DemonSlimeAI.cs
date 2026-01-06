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
    public Transform attackPoint;
    public float attackRadius = 0.8f;
    public LayerMask playerLayer;

    private bool canApplyDamage = false;
    private bool hasAppliedDamageThisSwing = false;

    [Header("SFX (Attack One Shot)")]
    public AudioClip attackSfx;
    [Range(0f, 1f)] public float attackSfxVolume = 1f;
    [Range(0f, 1f)] public float attackSfxSpatialBlend = 1f;
    [Tooltip("Opcional. Se vazio, tenta usar AudioSource no inimigo; se não houver, cria um temporário.")]
    public AudioSource sfxSource;

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

    [Header("SFX (Fireball One Shot)")]
    public AudioClip fireballSfx;
    [Range(0f, 1f)] public float fireballSfxVolume = 1f;
    [Tooltip("Opcional. Se vazio, tenta usar AudioSource no inimigo; se não houver, cria um temporário.")]
    public AudioSource fireballSfxSource;

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

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        if (fireballSfxSource == null)
            fireballSfxSource = sfxSource; // se quiseres usar o mesmo
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
            case State.Chasing: Chase(); break;
            case State.Attacking: Attack(); break;
        }

        if (animator != null)
        {
            bool shouldRun = (state == State.Chasing);
            animator.SetBool(runningBool, shouldRun);
        }

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
        currentDirection = Vector3.zero;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (animator != null)
                animator.SetTrigger(attackTrigger);

            // ✅ Som de ataque (quando inicia a animação)
            PlayAttackSfx();

            canApplyDamage = true;
            hasAppliedDamageThisSwing = false;

            lastAttackTime = Time.time;
        }
    }

    private void PlayAttackSfx()
    {
        if (attackSfx == null) return;

        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(attackSfx, attackSfxVolume);
            return;
        }

        GameObject go = new GameObject("AttackSFX_Temp");
        go.transform.position = transform.position;

        AudioSource temp = go.AddComponent<AudioSource>();
        temp.spatialBlend = attackSfxSpatialBlend;
        temp.playOnAwake = false;

        temp.PlayOneShot(attackSfx, attackSfxVolume);
        Destroy(go, attackSfx.length + 0.1f);
    }

    // (Animation Event) — chamar no frame do impacto
    public void OnAttackHit()
    {
        if (!canApplyDamage || hasAppliedDamageThisSwing) return;
        if (attackPoint == null)
        {
            Debug.LogWarning("AttackPoint não está definido no DemonSlimeAI.");
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

    // (Animation Event) — chamar no fim do ataque
    public void OnAttackEnd()
    {
        canApplyDamage = false;
    }

    void ShootFireball()
    {
        if (fireballPrefab != null && player != null)
        {
            Vector3 dir = (player.position - transform.position).normalized;

            GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);

            Rigidbody2D rb2d = fireball.GetComponent<Rigidbody2D>();
            if (rb2d != null)
                rb2d.linearVelocity = dir * fireballSpeed;

            lastFireballTime = Time.time;

            // ✅ Som da fireball
            PlayFireballSfx();
        }
    }

    private void PlayFireballSfx()
    {
        if (fireballSfx == null) return;

        if (fireballSfxSource != null)
        {
            fireballSfxSource.PlayOneShot(fireballSfx, fireballSfxVolume);
            return;
        }

        GameObject go = new GameObject("FireballSFX_Temp");
        go.transform.position = transform.position;

        AudioSource temp = go.AddComponent<AudioSource>();
        temp.spatialBlend = 0f; // mantém 2D como tinhas
        temp.playOnAwake = false;

        temp.PlayOneShot(fireballSfx, fireballSfxVolume);
        Destroy(go, fireballSfx.length + 0.1f);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log(name + " recebeu " + damage + " de dano! Vida atual: " + currentHealth);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            Invoke(nameof(RestoreColor), 0.1f);
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

        SceneManager.LoadScene("GameVictory");
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
