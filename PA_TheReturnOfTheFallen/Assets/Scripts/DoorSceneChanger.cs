using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSceneChanger : MonoBehaviour
{
    [SerializeField] private string TutorialCombatArea;
    [SerializeField] private string SpawnCombatArea; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSpawnData.nextSpawnPoint = SpawnCombatArea;

            SceneManager.LoadScene(TutorialCombatArea);
        }
    }
}
