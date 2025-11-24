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

    // ============================
    // NOVO — Freeze do Player
    // ============================
    [Header("Diálogo – Restrição de Movimento Automática")]
    public bool freezePlayer = false;   // congela apenas enquanto o diálogo decorre
    // ============================

    private bool isShowing = false;
    private int currentIndex = 0;

    // Para só acontecer uma vez
    private bool hasTriggeredOnce = false;


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

        if (hasTriggeredOnce) return;
        if (isShowing) return;

        if (dialoguePanel == null)
        {
            Debug.LogError("[DialogueTriggerUI] dialoguePanel NÃO está ligado!");
            return;
        }

        StartCoroutine(ShowDialogueRoutine(other));
    }

    private IEnumerator ShowDialogueRoutine(GameObject player)
    {
        isShowing = true;

        // ===============================
        // NOVO — Congelar o player
        // (congela até ao fim do diálogo)
        // ===============================
        if (freezePlayer)
            StartCoroutine(FreezePlayerDuringDialogue(player));
        // ===============================

        dialoguePanel.SetActive(true);

        foreach (Transform child in dialoguePanel.transform)
            child.gameObject.SetActive(false);

        currentIndex = 0;

        // ===== MULTI DIÁLOGOS =====
        if (dialoguesToShow != null && dialoguesToShow.Length > 0)
        {
            while (currentIndex < dialoguesToShow.Length)
            {
                ShowStep(currentIndex);
                yield return StartCoroutine(WaitForAdvance());
                currentIndex++;
            }
        }
        else
        {
            // ===== DIÁLOGO ÚNICO =====
            ShowSingleDialogue();
            yield return StartCoroutine(WaitForAdvance());
        }

        dialoguePanel.SetActive(false);
        isShowing = false;

        // NUNCA MAIS REPETE
        hasTriggeredOnce = true;
    }

    private void ShowStep(int index)
    {
        foreach (Transform child in dialoguePanel.transform)
            child.gameObject.SetActive(false);

        if (dialoguesToShow[index] != null)
            dialoguesToShow[index].SetActive(true);

        if (dialogueText != null)
        {
            string txt = null;

            if (overrideTexts != null &&
                index < overrideTexts.Length &&
                !string.IsNullOrWhiteSpace(overrideTexts[index]))
            {
                txt = overrideTexts[index];
            }
            else if (!string.IsNullOrWhiteSpace(overrideText))
            {
                txt = overrideText;
            }

            if (!string.IsNullOrEmpty(txt))
                dialogueText.text = txt;
        }
    }

    private void ShowSingleDialogue()
    {
        foreach (Transform child in dialoguePanel.transform)
            child.gameObject.SetActive(false);

        if (dialogueToShow != null)
            dialogueToShow.SetActive(true);

        if (!string.IsNullOrWhiteSpace(overrideText) && dialogueText != null)
            dialogueText.text = overrideText;
    }

    private IEnumerator WaitForAdvance()
    {
        yield return null;

        float timer = 0f;

        while (timer < showTime)
        {
            if (Input.GetKeyDown(KeyCode.E))
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }
    }


    // ==========================================
    // NOVO — Freeze automático enquanto dura diálogo
    // ==========================================
    private IEnumerator FreezePlayerDuringDialogue(GameObject player)
    {
        CharacterController cc = player.GetComponent<CharacterController>();
        Rigidbody rb = player.GetComponent<Rigidbody>();
        PlayerController pCtrl = player.GetComponent<PlayerController>(); // caso uses script próprio

        // DESLIGAR MOVIMENTO
        if (pCtrl != null) pCtrl.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Espera até que o diálogo termine (isShowing = false)
        while (isShowing)
            yield return null;

        // REATIVAR MOVIMENTO
        if (pCtrl != null) pCtrl.enabled = true;

        if (rb != null)
            rb.isKinematic = false;
    }
}
