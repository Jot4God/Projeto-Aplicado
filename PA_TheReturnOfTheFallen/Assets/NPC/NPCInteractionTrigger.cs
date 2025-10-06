using UnityEngine;

public class NPCInteractionTrigger : MonoBehaviour
{
    public NPCInteraction npcController; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            npcController.PlayerPerto = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            npcController.PlayerPerto = false;
    }
}
