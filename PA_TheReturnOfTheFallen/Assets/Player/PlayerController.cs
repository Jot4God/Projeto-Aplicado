using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public LayerMask terrainLayer;
    public Rigidbody rb;
    public SpriteRenderer sr;
    public bool instantStop = true;

    [Header("Attack")]
    public Transform attackPoint;
    public SpriteRenderer swordSprite;

    // ✅ NOVO: SpriteRenderer do SLASH (normalmente está no attackPoint)
    [Header("AttackPoint Visual (Slash)")]
    public SpriteRenderer attackPointSprite;
    [Tooltip("Se o slash ficar ao contrário num dos lados, ativa isto para inverter o flip.")]
    public bool invertAttackPointFlip = false;

    [Header("Animation")]
    public Animator animator;
    public string runSideBool = "isRunningSide";
    public string runBackBool = "isRunningBack";
    public string runFrontBool = "isRunningFront";

    [HideInInspector]
    public bool podeMover = true;

    private Vector3 attackPointLocalStart;
    private int facingDirection = 1;
    private Vector3 inputDir;

    [Header("Step Climbing")]
    public float stepHeight = 0.4f;     // altura do degrau
    public float stepSmooth = 0.2f;     // suavidade ao subir

    // ✅ FIX: lock de facing durante ataques
    private bool facingLocked = false;

    // ✅ expor direção atual
    public int FacingDirection => facingDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (attackPoint != null)
            attackPointLocalStart = attackPoint.localPosition;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // ✅ NOVO: apanhar automaticamente o SpriteRenderer do attackPoint (slash)
        if (attackPointSprite == null && attackPoint != null)
            attackPointSprite = attackPoint.GetComponent<SpriteRenderer>();

        PlayerController[] players = Object.FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        if (players.Length > 1) { Destroy(gameObject); return; }
    }

    void Update()
    {
        if (!podeMover)
        {
            inputDir = Vector3.zero;
            UpdateAnimations(0, 0);
            return;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        inputDir = new Vector3(x, 0f, z).normalized;

        UpdateAnimations(x, z);

        // ✅ FIX: só permite flip se NÃO estiver locked
        if (!facingLocked)
            UpdateFlipAndAttackPoint(x);
    }

    void FixedUpdate()
    {
        HandleMovement();
        StepClimb();
    }

    // -------------------------
    //        MOVIMENTO
    // -------------------------
    void HandleMovement()
    {
        if (podeMover && inputDir.sqrMagnitude > 0f)
        {
            Vector3 desired = inputDir * speed;
            Vector3 vel = rb.linearVelocity;
            rb.linearVelocity = new Vector3(desired.x, vel.y, desired.z);
        }
        else
        {
            if (instantStop)
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    // -------------------------
    //     STEP CLIMB (SUBIR DEGRAUS)
    // -------------------------
    void StepClimb()
    {
        Vector3 dir = new Vector3(inputDir.x, 0f, inputDir.z);
        if (dir == Vector3.zero) return;

        if (Physics.Raycast(transform.position, dir, out RaycastHit hitLower, 0.5f))
        {
            Vector3 upperOrigin = transform.position + Vector3.up * stepHeight;

            if (!Physics.Raycast(upperOrigin, dir, 0.5f))
                rb.position += Vector3.up * stepSmooth;
        }
    }

    // -------------------------
    //       ANIMAÇÕES
    // -------------------------
    void UpdateAnimations(float x, float z)
    {
        if (animator == null) return;

        bool isRunningSide = Mathf.Abs(x) > 0.0001f;
        bool isRunningBack = z > 0.0001f;
        bool isRunningFront = z < -0.0001f;

        animator.SetBool(runSideBool, isRunningSide);
        animator.SetBool(runBackBool, isRunningBack);
        animator.SetBool(runFrontBool, isRunningFront);
    }

    // -------------------------
    //     FLIP + ATTACK POINT
    // -------------------------
    void UpdateFlipAndAttackPoint(float x)
    {
        if (Mathf.Abs(x) > 0.0001f)
        {
            facingDirection = x > 0 ? 1 : -1;
            ApplyFacingToVisuals();
        }
    }

    // ✅ aplicar visuals a partir do facingDirection atual
    private void ApplyFacingToVisuals()
    {
        // Player sprite
        if (sr != null)
            sr.flipX = facingDirection < 0;

        // Attack point posiciona para o lado certo
        if (attackPoint != null)
        {
            Vector3 lp = attackPointLocalStart;
            lp.x = Mathf.Abs(attackPointLocalStart.x) * facingDirection;
            attackPoint.localPosition = lp;

            // manter consistente (evita rotações acumuladas)
            attackPoint.localEulerAngles = Vector3.zero;

            // manter escala positiva (o flip fica só no SpriteRenderer)
            Vector3 scale = attackPoint.localScale;
            scale.x = Mathf.Abs(scale.x);
            attackPoint.localScale = scale;
        }

        // Sword sprite (se usares)
        if (swordSprite != null)
            swordSprite.flipX = facingDirection < 0;

        // ✅ NOVO: flip do slash (SpriteRenderer do attackPoint)
        if (attackPointSprite != null)
        {
            bool flip = (facingDirection < 0);
            if (invertAttackPointFlip) flip = !flip;
            attackPointSprite.flipX = flip;
        }
    }

    // ✅ API para o PlayerAttack bloquear/desbloquear
    public void LockFacing(int dir)
    {
        facingLocked = true;
        facingDirection = (dir >= 0) ? 1 : -1;
        ApplyFacingToVisuals();
    }

    public void UnlockFacing()
    {
        facingLocked = false;
    }
}
