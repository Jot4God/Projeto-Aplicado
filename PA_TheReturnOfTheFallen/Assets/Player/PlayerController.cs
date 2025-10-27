using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float groundDist = 1f;
    public LayerMask terrainLayer;
    public Rigidbody rd;
    public SpriteRenderer sr;

    [Header("Attack")]
    public Transform attackPoint;     // referência ao AttackPoint (filho do Player)
    public SpriteRenderer swordSprite; // 👈 sprite da espada (filho do attackPoint)

    [HideInInspector] public bool podeMover = true;

    private Vector3 attackPointLocalStart;
    private int facingDirection = 1; // 1 = direita, -1 = esquerda

    void Start()
    {
        rd = GetComponent<Rigidbody>();
        rd.constraints = RigidbodyConstraints.FreezeRotation;

        if (attackPoint != null)
            attackPointLocalStart = attackPoint.localPosition;
    }

    void Update()
    {
        if (!podeMover)
        {
            rd.linearVelocity = Vector3.zero;
            return;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(x, 0, z).normalized;
        rd.linearVelocity = new Vector3(moveDir.x * speed, rd.linearVelocity.y, moveDir.z * speed);

        // Atualiza direção e gira o attackPoint
        if (attackPoint != null && Mathf.Abs(x) > 0.0001f)
        {
            facingDirection = x > 0 ? 1 : -1;

            // inverte a posição local do attackPoint
            Vector3 lp = attackPointLocalStart;
            lp.x = Mathf.Abs(attackPointLocalStart.x) * facingDirection;
            attackPoint.localPosition = lp;

            // força rotação/escala corretas no attackPoint
            attackPoint.localEulerAngles = Vector3.zero;
            Vector3 scale = attackPoint.localScale;
            scale.x = Mathf.Abs(scale.x);
            attackPoint.localScale = scale;

            // 👇 flip da sprite da sword
            if (swordSprite != null)
                swordSprite.flipX = facingDirection < 0;
        }
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, Mathf.Infinity, terrainLayer))
        {
            float targetY = hit.point.y + groundDist;
            Vector3 pos = rd.position;
            pos.y = Mathf.Lerp(pos.y, targetY, 0.2f);
            rd.MovePosition(pos);
        }
    }

    void Awake()
    {
        PlayerController[] players = Object.FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

        if (players.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
