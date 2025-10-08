using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;  // Seu inimigo
    public int numberOfEnemies = 5; // Quantos inimigos spawnar no total
    public float spawnRadius = 5f;  // Raio máximo de spawn ao redor do player
    public float minDistanceFromPlayer = 3f; // Distância mínima do player para spawn seguro

    private Transform player;

    void Start()
    {
        // Procura o player na cena
        player = GameObject.FindWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogWarning("Player não encontrado! Verifique se ele tem a tag 'Player'.");
            return;
        }

        // Spawn inicial de inimigos
        for (int i = 0; i < numberOfEnemies; i++)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (player == null) return;

        Vector3 spawnPos;
        int tries = 0;

        // Garante que o inimigo não spawn no player
        do
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                0,
                Random.Range(-spawnRadius, spawnRadius)
            );

            spawnPos = player.position + randomOffset;
            tries++;

            // Evita loop infinito caso não consiga achar posição
            if (tries > 20)
                break;

        } while (Vector3.Distance(spawnPos, player.position) < minDistanceFromPlayer);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}
