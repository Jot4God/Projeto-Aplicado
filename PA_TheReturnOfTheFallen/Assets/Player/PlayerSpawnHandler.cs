using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnHandler : MonoBehaviour
{
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
            }
            else
            {
                Debug.LogWarning($"Spawn point '{PlayerSpawnData.nextSpawnPoint}' não encontrado na cena '{scene.name}'");
            }
        }
    }
}
