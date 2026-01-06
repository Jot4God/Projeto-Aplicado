using UnityEngine;

public class NPCInteractionTrigger : MonoBehaviour
{
    [Header("NPC")]
    public NPCInteraction npcController;

    [Header("Ambiente temporário (quando estás perto)")]
    public bool changeAmbientWhileInside = true;
    public AudioClip npcAmbientClip;
    [Range(0f, 1f)] public float npcAmbientVolume = 1f;

    [Header("Bloquear ataque enquanto está no trigger")]
    public bool blockPlayerAttackWhileInside = true;
    public MonoBehaviour playerAttackScript; // arrasta aqui o script de ataque do Player (ex: PlayerAttack, PlayerCombat, etc.)

    // Guardar o que estava antes
    private AudioClip previousClip;
    private float previousVolume;
    private bool hasPrevious;

    // Guardar estado anterior do ataque
    private bool attackWasEnabled;
    private bool hasAttackPrevious;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        npcController.PlayerPerto = true;

        // Bloquear ataque
        if (blockPlayerAttackWhileInside)
        {
            if (playerAttackScript == null)
                playerAttackScript = other.GetComponent<MonoBehaviour>(); // tenta apanhar algo, mas o ideal é arrastares no Inspector

            if (playerAttackScript != null && !hasAttackPrevious)
            {
                attackWasEnabled = playerAttackScript.enabled;
                hasAttackPrevious = true;
                playerAttackScript.enabled = false;
            }
        }

        if (!changeAmbientWhileInside) return;
        if (AmbientPlayer.Instance == null) return;
        if (npcAmbientClip == null) return;

        // Guardar o ambiente atual só na primeira entrada
        if (!hasPrevious)
        {
            previousClip = AmbientPlayer.Instance.CurrentClip;
            previousVolume = AmbientPlayer.Instance.CurrentVolume;
            hasPrevious = true;
        }

        // Trocar para a música do NPC
        AmbientPlayer.Instance.PlayAmbient(npcAmbientClip, npcAmbientVolume);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        npcController.PlayerPerto = false;

        // Repor ataque
        if (blockPlayerAttackWhileInside && playerAttackScript != null && hasAttackPrevious)
        {
            playerAttackScript.enabled = attackWasEnabled;
        }
        hasAttackPrevious = false;

        if (!changeAmbientWhileInside) return;
        if (AmbientPlayer.Instance == null) return;

        // Voltar ao que estava
        if (hasPrevious && previousClip != null)
        {
            AmbientPlayer.Instance.PlayAmbient(previousClip, previousVolume);
        }

        hasPrevious = false;
        previousClip = null;
        previousVolume = 1f;
    }
}
