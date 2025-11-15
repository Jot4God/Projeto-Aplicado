using UnityEngine;

public class FireballScript : MonoBehaviour
{
    public int damage = 20;
    public float speed = 8f;
    public float lifetime = 3f;
    public Animator animator;

    private Rigidbody rb;
    private Vector3 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Se o player existir, aponta para ele
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            direction = (player.transform.position - transform.position).normalized;
        }
        else
        {
            direction = transform.forward; // fallback
        }

        // Movimento 3D
        rb.linearVelocity = direction * speed;

        // A animação já começa automaticamente (Entry → Spell)

        // Auto-destruição
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider collision)
    {
        // ----- ACERTA O PLAYER -----
        if (collision.CompareTag("Player"))
        {
            PlayerHP hp = collision.GetComponentInParent<PlayerHP>();
            if (hp != null)
                hp.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }

    }
}