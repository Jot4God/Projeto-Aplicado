using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public int maxRevives = 3;
    public int currentRevives;

    [HideInInspector]
    public RespawnZone currentZone;

    private PlayerHP hp;
    private Rigidbody rb;

    private void Awake()
    {
        currentRevives = maxRevives;
        hp = GetComponent<PlayerHP>();
        rb = GetComponent<Rigidbody>();
    }

    public void HandleDeath()
    {
        if (currentRevives <= 0)
        {
            // sem revives → game over original
            hp.ForceGameOver();
            return;
        }

        currentRevives--;

        // Atualiza UI de revives
        Object.FindFirstObjectByType<PlayerEquipmentUI>()?.UpdateRevivesUI(currentRevives);

RespawnPlayer();
    }

private void RespawnPlayer()
{
    if (currentZone == null || currentZone.respawnPoint == null)
    {
        Debug.LogWarning("Sem zona de respawn definida.");
        hp.ForceGameOver();
        return;
    }

    Transform point = currentZone.respawnPoint;

    // COMPONENTES DO PLAYER
    var rb = GetComponent<Rigidbody>();
    var cc = GetComponent<CharacterController>();
    var controller = GetComponent<PlayerController>(); // ou o nome do teu script de movimento

    // 1. DESATIVAR MOVIMENTO E FÍSICA
    if (controller) controller.enabled = false;
    if (rb)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }
    if (cc) cc.enabled = false;

    // 2. TELEPORT ABSOLUTO
    transform.position = point.position;
    transform.rotation = point.rotation;

    // 3. REATIVAR TUDO NO FRAME SEGUINTE
    StartCoroutine(ReenableComponentsNextFrame(controller, rb, cc));

    // 4. RESET HP
    hp.currentHealth = hp.maxHealth;
    hp.UpdateUIInstant();

    Debug.Log("RESPAWN NO EXIT POINT: " + point.position);
}

private System.Collections.IEnumerator ReenableComponentsNextFrame(
    PlayerController controller, Rigidbody rb, CharacterController cc)
{
    yield return null; // ESPERA 1 FRAME

    if (rb) rb.isKinematic = false;
    if (cc) cc.enabled = true;
    if (controller) controller.enabled = true;
}

}
