using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueTriggerUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;        // painel geral (pai)
    public GameObject dialogueToShow;       // texto específico a mostrar (um dos filhos)
    public TextMeshProUGUI dialogueText;    // opcional, só se quiseres mudar o texto

    [Header("Opções")]
    [TextArea(2, 4)]
    public string overrideText = "";        // se deixares vazio, usa o texto já no TMP
    public float showTime = 5f;

    private bool isShowing = false;

    // 3D
    private void OnTriggerEnter(Collider other)
    {
        TryStartDialogue(other.gameObject);
    }

    // 2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryStartDialogue(other.gameObject);
    }

    private void TryStartDialogue(GameObject other)
    {
        if (!other.CompareTag("Player")) return;
        if (isShowing) return;
        if (dialoguePanel == null)
        {
            Debug.LogError("[DialogueTriggerUI] dialoguePanel NÃO está ligado!");
            return;
        }

        StartCoroutine(ShowDialogueRoutine());
    }

    private IEnumerator ShowDialogueRoutine()
    {
        isShowing = true;

        // ativa o painel principal
        dialoguePanel.SetActive(true);

        // desativa todos os filhos primeiro (garante que só um fica ativo)
        foreach (Transform child in dialoguePanel.transform)
            child.gameObject.SetActive(false);

        // ativa apenas o diálogo escolhido
        if (dialogueToShow != null)
            dialogueToShow.SetActive(true);

        // se quiseste substituir o texto
        if (!string.IsNullOrWhiteSpace(overrideText) && dialogueText != null)
            dialogueText.text = overrideText;

        yield return new WaitForSeconds(showTime);

        // desliga tudo
        dialoguePanel.SetActive(false);
        isShowing = false;
    }
}
