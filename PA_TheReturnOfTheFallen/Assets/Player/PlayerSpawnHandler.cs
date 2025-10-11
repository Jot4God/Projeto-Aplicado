using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnHandler : MonoBehaviour
{
    public Vector3 newPlayerScale = new Vector3(1f, 1f, 1f);
    public float speedIncrease = 1f; 

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!string.IsNullOrEmpty(PlayerSpawnData.nextSpawnPoint))
        {
            GameObject spawnPoint = GameObject.Find(PlayerSpawnData.nextSpawnPoint);

            if (spawnPoint != null)
            {
                transform.position = spawnPoint.transform.position;

                transform.localScale = newPlayerScale;

                PlayerController pc = GetComponent<PlayerController>();
                if (pc != null)
                {
                    pc.speed += speedIncrease;
                    Debug.Log($"Velocidade aumentada para {pc.speed}");
                }
                else
                {
                    Debug.LogWarning("PlayerController não encontrado no jogador!");
                }
            }
            else
            {
                Debug.LogWarning($"Spawn point '{PlayerSpawnData.nextSpawnPoint}' não encontrado na cena '{scene.name}'");
            }
        }
    }
}
