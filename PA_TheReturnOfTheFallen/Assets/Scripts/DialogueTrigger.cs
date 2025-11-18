using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueTriggerUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;

    [Tooltip("Se usar sequência, preenche esta lista com os filhos na ordem desejada.")]
    public GameObject[] dialoguesToShow;

    [Tooltip("Para modo antigo (apenas 1 diálogo). Se a lista de cima estiver vazia, usa isto.")]
    public GameObject dialogueToShow;

    public TextMeshProUGUI dialogueText;

    [Header("Textos (opcional)")]
    [TextArea(2, 4)]
    public string overrideText = "";

    [TextArea(2, 4)]
    public string[] overrideTexts;

    [Header("Opções")]
    public float showTime = 5f;

    private bool isShowing = false;
    private int currentIndex = 0;

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
        dialoguePanel.SetActive(true);

        // desativa todos os filhos do painel
        foreach (Transform child in dialoguePanel.transform)
            child.gameObject.SetActive(false);

        // reinicia índice
        currentIndex = 0;

        // ======== MODO NOVO: vários diálogos =========
        if (dialoguesToShow != null && dialoguesToShow.Length > 0)
        {
            while (currentIndex < dialoguesToShow.Length)
            {
                ShowStep(currentIndex);

                // Espera tempo ou tecla E (robusto)
                yield return StartCoroutine(WaitForAdvance());

                // avança para o próximo elemento da sequência
                currentIndex++;
            }
        }
        // ======== MODO ANTIGO: apenas 1 diálogo =========
        else
        {
            ShowSingleDialogue();

            // espera tempo ou tecla E
            yield return StartCoroutine(WaitForAdvance());
        }

        // FECHAR PAINEL quando acabar todos
        dialoguePanel.SetActive(false);
        isShowing = false;
    }

    /// Mostra um diálogo específico da sequência (index em ordem)
    private void ShowStep(int index)
    {
        // desativa todos os filhos do painel (garantia)
        foreach (Transform child in dialoguePanel.transform)
            child.gameObject.SetActive(false);

        if (dialoguesToShow[index] != null)
            dialoguesToShow[index].SetActive(true);

        if (dialogueText != null)
        {
            string textToUse = null;

            if (overrideTexts != null &&
                index < overrideTexts.Length &&
                !string.IsNullOrWhiteSpace(overrideTexts[index]))
            {
                textToUse = overrideTexts[index];
            }
            else if (!string.IsNullOrWhiteSpace(overrideText))
            {
                textToUse = overrideText;
            }

            if (!string.IsNullOrEmpty(textToUse))
                dialogueText.text = textToUse;
        }
    }

    /// Mostra o diálogo único (modo antigo)
    private void ShowSingleDialogue()
    {
        foreach (Transform child in dialoguePanel.transform)
            child.gameObject.SetActive(false);

        if (dialogueToShow != null)
            dialogueToShow.SetActive(true);

        if (!string.IsNullOrWhiteSpace(overrideText) && dialogueText != null)
            dialogueText.text = overrideText;
    }

    /// Espera até o jogador carregar E OU até o tempo showTime expirar.
    /// Faz um frame de tolerância para ignorar qualquer input que tenha acontecido
    /// na mesma frame em que o diálogo foi ativado.
    private IEnumerator WaitForAdvance()
    {
        // ignora input da mesma frame em que a coroutine começou
        yield return null;

        float timer = 0f;

        while (timer < showTime)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                yield break; // avança já
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // tempo esgotado -> apenas retorna e permite avançar
    }
}
