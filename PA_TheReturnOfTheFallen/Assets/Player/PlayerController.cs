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

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (attackPoint != null)
            attackPointLocalStart = attackPoint.localPosition;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

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
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
        }
    }

    // -------------------------
    //     STEP CLIMB (SUBIR DEGRAUS)
    // -------------------------
    void StepClimb()
    {
        Vector3 dir = new Vector3(inputDir.x, 0f, inputDir.z);
        if (dir == Vector3.zero) return;

        // Ray baixo (detecta parede)
        if (Physics.Raycast(transform.position, dir, out RaycastHit hitLower, 0.5f))
        {
            // Ray alto (acima do pé)
            Vector3 upperOrigin = transform.position + Vector3.up * stepHeight;

            if (!Physics.Raycast(upperOrigin, dir, 0.5f))
            {
                rb.position += Vector3.up * stepSmooth;
            }
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
            int dir = x > 0 ? 1 : -1;

            if (sr != null)
                sr.flipX = dir < 0;

            if (attackPoint != null)
            {
                facingDirection = dir;

                Vector3 lp = attackPointLocalStart;
                lp.x = Mathf.Abs(attackPointLocalStart.x) * facingDirection;
                attackPoint.localPosition = lp;

                attackPoint.localEulerAngles = Vector3.zero;

                Vector3 scale = attackPoint.localScale;
                scale.x = Mathf.Abs(scale.x);
                attackPoint.localScale = scale;

                if (swordSprite != null)
                    swordSprite.flipX = facingDirection < 0;
            }
        }
    }
}
