using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuração de Spawn")]
    public GameObject enemyPrefab;          // Prefab do inimigo
    public int numberOfEnemies = 5;         // Quantos inimigos spawnar
    public float spawnRadius = 5f;          // Raio ao redor do spawner
    public float spawnDelay = 1f;           // Tempo entre spawns

    [Header("Detecção do Player")]
    public float activationRadius = 10f;    // Distância que ativa o spawner
    private Transform player;

    [Header("Altura fixa de spawn")]
    public float spawnHeight = 1f;          // Altura Y fixa para spawn (acima do terreno)

    private bool hasSpawned = false;        // Evita spawn repetido

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogWarning("⚠️ Player não encontrado! Verifique se ele tem a tag 'Player'.");
        }
    }

    void Update()
    {
        if (player == null || hasSpawned) return;

        // Ativa o spawn quando o player entra no raio
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer <= activationRadius)
        {
            StartCoroutine(SpawnEnemiesWithDelay());
            hasSpawned = true;
        }
    }

    IEnumerator SpawnEnemiesWithDelay()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnEnemy()
    {
        // Gera posição aleatória ao redor do spawner
        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            0,
            Random.Range(-spawnRadius, spawnRadius)
        );

        // Define a posição final (mantendo o Y fixo)
        Vector3 spawnPos = transform.position + randomOffset;
        spawnPos.y = spawnHeight; // Mantém o Y fixo em 1 (ou valor definido no Inspector)

        // Cria o inimigo
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        // Mostra o raio de ativação e de spawn no editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activationRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
