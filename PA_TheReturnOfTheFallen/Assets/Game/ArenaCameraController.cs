using UnityEngine;

public class ArenaCameraController : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 5f;
    public float lookAheadDistance = 2f;

    private Vector3 offset;

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

        if (player != null)
            offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 lookAhead = Vector3.zero;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 dir = rb.linearVelocity.normalized;
            lookAhead = (Vector3)dir * lookAheadDistance;
        }

        // posição alvo (agora inclui Z do jogador)
        Vector3 targetPos = player.position + offset + lookAhead;

        // movimento suave
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}
