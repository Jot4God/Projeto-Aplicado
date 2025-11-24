using UnityEngine;

public class RespawnZone : MonoBehaviour
{
    [Header("Respawn para esta zona")]
    public Transform respawnPoint; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var respawn = other.GetComponent<PlayerRespawn>();

            // Só muda de zona quando ENTRA noutra
            if (respawn != null)
                respawn.currentZone = this;
        }
    }

    // ⚠️ IMPORTANTE:
    // Não apagamos a zona ao sair, porque isso causa o bug de respawn inconsistente.
    // O jogador mantém a última zona válida até entrar noutra.

    private void OnTriggerExit(Collider other)
    {
        // intencionalmente vazio
    }
}
