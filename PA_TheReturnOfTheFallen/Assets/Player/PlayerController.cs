using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float groundDist = 0.02f;   // distância mínima ao chão
    public LayerMask terrainLayer;
    public Rigidbody rb;
    public SpriteRenderer sr;
    public bool instantStop = true; // parar seco ao largar as teclas

    [Header("Ground Force")]
    public float groundForce = 40f;   // 🔥 força para puxar o player para baixo

    [Header("Attack")]
    public Transform attackPoint;
    public SpriteRenderer swordSprite;

    [Header("Animation")]
    public Animator animator;
    public string runSideBool = "isRunningSide";
    public string runBackBool = "isRunningBack";
    public string runFrontBool = "isRunningFront";

    [HideInInspector] public bool podeMover = true;

    private Vector3 attackPointLocalStart;
    private int facingDirection = 1;
    private Vector3 inputDir;

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

            if (animator != null)
            {
                animator.SetBool(runSideBool, false);
                animator.SetBool(runBackBool, false);
                animator.SetBool(runFrontBool, false);
            }

            return;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        inputDir = new Vector3(x, 0f, z).normalized;

        if (animator != null)
        {
            bool isRunningSide  = Mathf.Abs(x) > 0.0001f;
            bool isRunningBack  = z > 0.0001f;
            bool isRunningFront = z < -0.0001f;

            animator.SetBool(runSideBool,  isRunningSide);
            animator.SetBool(runBackBool,  isRunningBack);
            animator.SetBool(runFrontBool, isRunningFront);
        }

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

    void FixedUpdate()
    {
        // movimento
        if (podeMover)
        {
            if (inputDir.sqrMagnitude > 0f)
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
        else
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }

        // 🔥 RAYCAST para saber onde está o chão
        Vector3 rayOrigin = transform.position + Vector3.up * 5f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 20f, terrainLayer))
        {
            float targetY = hit.point.y + groundDist;
            float diff = rb.position.y - targetY;

            // 🔥 Se o player está acima do chão → puxa para baixo (GROUND FORCE)
            if (diff > 0.02f)
            {
                Vector3 vel = rb.linearVelocity;
                vel.y = -groundForce;  // força de "gravidade" rápida
                rb.linearVelocity = vel;
            }
            else
            {
                // cola ao chão
                Vector3 pos = rb.position;
                pos.y = targetY;
                rb.MovePosition(pos);

                // zera velocidade vertical
                Vector3 vel = rb.linearVelocity;
                vel.y = 0f;
                rb.linearVelocity = vel;
            }
        }
    }
}
