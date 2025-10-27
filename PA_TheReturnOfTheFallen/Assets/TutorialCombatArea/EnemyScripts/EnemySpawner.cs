using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Inimigo base (na cena, não prefab)")]
    [Tooltip("Arrasta aqui o slime principal que está na cena (com HideOnPlay).")]
    public GameObject sceneEnemy;           // Slime principal da cena

    [Header("Configuração de Spawn")]
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

        if (sceneEnemy == null)
        {
            Debug.LogError("❌ Falta arrastar o slime principal para o campo 'Scene Enemy' no spawner.");
        }
    }

    void Update()
    {
        if (player == null || hasSpawned || sceneEnemy == null) return;

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
            0f,
            Random.Range(-spawnRadius, spawnRadius)
        );

        Vector3 spawnPos = transform.position + randomOffset;
        spawnPos.y = spawnHeight;

        // ✅ Clona o inimigo base mesmo que esteja desativado
        GameObject clone = Instantiate(sceneEnemy, spawnPos, Quaternion.identity);

        // Ativa o clone (já que o template está desativado)
        if (!clone.activeSelf) clone.SetActive(true);

        // ✅ Reatribui o Player (para follow e ataque)
        EnemyController ec = clone.GetComponent<EnemyController>();
        if (ec != null && ec.player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) ec.player = p.transform;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activationRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
