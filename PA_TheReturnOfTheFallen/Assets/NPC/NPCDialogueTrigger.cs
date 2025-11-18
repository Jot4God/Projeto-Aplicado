using UnityEngine;
using TMPro;

public class NPCDialogueTrigger : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;        // O painel de diálogo que será ativado
    public TextMeshProUGUI dialogueText;    // O texto do diálogo a ser mostrado
    public string[] dialogues;              // Array com os diálogos que serão exibidos

    [Header("Opções")]
    public float interactionRange = 3f;     // Distância em que o jogador pode interagir com o NPC
    private bool isInRange = false;         // Flag para verificar se o jogador está dentro do range
    private bool isDialogueActive = false;  // Flag para verificar se o diálogo está ativo
    private int currentDialogueIndex = 0;   // Índice do diálogo atual

    private void Update()
    {
        // Quando o jogador está no range e pressiona 'E', ativa ou avança no diálogo
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (isDialogueActive)
            {
                SkipDialogue();
            }
            else
            {
                StartDialogue();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o jogador entrou na área de interação
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            Debug.Log("Jogador entrou na área de interação.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Verifica quando o jogador sai do range
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            Debug.Log("Jogador saiu da área de interação.");
            if (isDialogueActive)
            {
                CloseDialogue();
            }
        }
    }

    private void StartDialogue()
    {
        // Começa o diálogo se houver diálogos configurados
        if (dialogues.Length == 0)
        {
            Debug.LogError("Nenhum diálogo foi configurado!");
            return;
        }

        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        currentDialogueIndex = 0;
        ShowDialogue();
    }

    private void ShowDialogue()
    {
        // Exibe o diálogo atual
        if (currentDialogueIndex < dialogues.Length)
        {
            dialogueText.text = dialogues[currentDialogueIndex];
        }
        else
        {
            CloseDialogue();
        }
    }

    private void SkipDialogue()
    {
        // Avança para o próximo diálogo
        currentDialogueIndex++;
        ShowDialogue();
    }

    private void CloseDialogue()
    {
        // Fecha o painel de diálogo
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        Debug.Log("Diálogo fechado.");
    }

    // Gizmo para visualizar o range de interação do NPC
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
