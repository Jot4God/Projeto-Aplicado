using UnityEngine;

public class HealthBarPersistent : MonoBehaviour
{
    private static HealthBarPersistent instance;

    void Awake()
    {
        // Se já existir uma instância, destrói o duplicado
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Define esta como a instância única
        instance = this;


    }
}
