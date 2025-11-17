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

    [Header("Attack")]
    public Transform attackPoint;       // filho do Player
    public SpriteRenderer swordSprite;  // sprite da espada (filho do attackPoint)

    [Header("Animation")]
    public Animator animator;           // podes deixar vazio no Inspector
    public string runSideBool = "isRunningSide";
    public string runBackBool = "isRunningBack";   // W
    public string runFrontBool = "isRunningFront"; // S

    [HideInInspector] public bool podeMover = true;

    // privados
    private Vector3 attackPointLocalStart;
    private int facingDirection = 1; // 1 = direita, -1 = esquerda
    private Vector3 inputDir;        // guardado em Update (−1/0/1)

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate; // suaviza visualmente

        if (attackPoint != null)
            attackPointLocalStart = attackPoint.localPosition;

        // se não ligaste o Animator no inspector, tenta apanhar num filho
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // opção: persistência
        PlayerController[] players = Object.FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        if (players.Length > 1) { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
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

        // input
        float x = Input.GetAxisRaw("Horizontal"); // -1,0,1  (A/D)
        float z = Input.GetAxisRaw("Vertical");   // -1,0,1  (S/W por defeito: W=+1, S=-1)
        inputDir = new Vector3(x, 0f, z).normalized;

        // animações de correr
        if (animator != null)
        {
            bool isRunningSide  = Mathf.Abs(x) > 0.0001f;   // só A/D
            bool isRunningBack  = z > 0.0001f;              // W (para trás nas tuas animações)
            bool isRunningFront = z < -0.0001f;             // S (para a frente)

            animator.SetBool(runSideBool,  isRunningSide);
            animator.SetBool(runBackBool,  isRunningBack);
            animator.SetBool(runFrontBool, isRunningFront);
        }

        // FLIP do sprite do player (andar para a esquerda mostra virado à esquerda)
        if (Mathf.Abs(x) > 0.0001f)
        {
            int dir = x > 0 ? 1 : -1;

            if (sr != null)
                sr.flipX = dir < 0;   // esquerda = flip

            // Atualiza direção e gira o attackPoint
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

        // 🔥 COLAR AO CHÃO SEMPRE
        // origem bem acima do player para não bater em paredes/objetos à frente
        Vector3 rayOrigin = transform.position + Vector3.up * 5f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 20f, terrainLayer))
        {
            // posição exata no chão + offset pequeno
            float targetY = hit.point.y + groundDist;

            Vector3 pos = rb.position;
            pos.y = targetY;              // sem Lerp -> cola mesmo
            rb.MovePosition(pos);
        }
    }
}
