using UnityEngine;

public class FireballScript : MonoBehaviour
{
    public int damage = 20;            // Dano que a fireball causa ao jogador
    public float speed = 10f;          // Velocidade da fireball
    public float lifetime = 5f;        // Tempo de vida da fireball antes de desaparecer
    public Animator animator;          // Animator para animação da fireball

    private Rigidbody rb;
    private Vector3 direction;         // Direção da fireball

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>(); // Pega o Animator associado ao prefab

        // Verifica se o jogador está presente e pega a posição do player
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            // Calcula a direção da fireball para o player
            direction = (player.position - transform.position).normalized;
        }
        else
        {
            direction = transform.forward; // fallback, caso não encontre o player (ex: para outros movimentos)
        }

        // Define a velocidade e direção do movimento (Rigidbody 3D)
        rb.linearVelocity = direction * speed;

        // A animação é reproduzida enquanto a fireball está em movimento.
        if (animator != null)
        {
            animator.Play("_SpellAnim");  // Inicia a animação "Spell" diretamente
        }

        // Destroi a fireball depois de um tempo (para não ficar no mundo para sempre)
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Se a fireball colidir com o jogador
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHP playerHP = collision.gameObject.GetComponent<PlayerHP>();
            if (playerHP != null)
            {
                playerHP.TakeDamage(damage); // Aplica o dano ao jogador
            }

            // Destroi a fireball após atingir o jogador
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // Se atingir outro inimigo, também aplica o dano
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(damage);
            }

            // Destroi a fireball
            Destroy(gameObject);
        }
        else
        {
            // Opcional: destruir ao bater em cenário
            Destroy(gameObject);
        }
    }
}
