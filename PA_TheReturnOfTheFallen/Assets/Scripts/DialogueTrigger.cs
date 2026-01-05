using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueTriggerUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;

    public GameObject[] dialoguesToShow;
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
    // SOM (ONE-SHOT)
    // ============================
    [Header("Som (One Shot ao abrir diálogo)")]
    [Tooltip("Som a tocar uma única vez quando o diálogo abre.")]
    public AudioClip openDialogueSfx;

    [Range(0f, 1f)]
    public float openDialogueSfxVolume = 1f;

    [Tooltip("Se vazio, usa AudioSource no próprio objeto; se não houver, cria um temporário.")]
    public AudioSource sfxSource;

    // ============================
    // FREEZE
    // ============================
    [Header("Freeze do Player")]
    public bool freezePlayer = false;

    [Tooltip("Nome EXATO do estado Idle")]
    public string idleStateName = "Idle";

    private bool isShowing = false;
    private bool hasTriggeredOnce = false;
    private int currentIndex = 0;

    // ============================
    // TRIGGERS
    // ============================
    private void OnTriggerEnter(Collider other)
    {
        TryStartDialogue(other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryStartDialogue(other.gameObject);
    }

    private void TryStartDialogue(GameObject other)
    {
        if (!other.CompareTag("Player")) return;
        if (hasTriggeredOnce || isShowing) return;

        StartCoroutine(ShowDialogueRoutine(other));
    }

    // ============================
    // ROTINA PRINCIPAL
    // ============================
    private IEnumerator ShowDialogueRoutine(GameObject player)
    {
        isShowing = true;

        // ✅ Toca 1 vez ao abrir o diálogo (não mexe no ambient)
        PlayOpenDialogueSfx();

        if (freezePlayer)
            StartCoroutine(FreezePlayerDuringDialogue(player));

        dialoguePanel.SetActive(true);

        foreach (Transform child in dialoguePanel.transform)
            child.gameObject.SetActive(false);

        currentIndex = 0;

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
            ShowSingleDialogue();
            yield return StartCoroutine(WaitForAdvance());
        }

        dialoguePanel.SetActive(false);
        isShowing = false;
        hasTriggeredOnce = true;
    }

    // ============================
    // SOM (ONE-SHOT)
    // ============================
    private void PlayOpenDialogueSfx()
    {
        if (openDialogueSfx == null) return;

        // Se não atribuíram um source, tenta obter do objeto
        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(openDialogueSfx, openDialogueSfxVolume);
            return;
        }

        // Fallback: cria um AudioSource temporário
        GameObject go = new GameObject("DialogueSFX_Temp");
        go.transform.position = transform.position;

        AudioSource temp = go.AddComponent<AudioSource>();
        temp.spatialBlend = 0f; // 2D
        temp.playOnAwake = false;
        temp.volume = 1f;

        temp.PlayOneShot(openDialogueSfx, openDialogueSfxVolume);
        Destroy(go, openDialogueSfx.length + 0.1f);
    }

    // ============================
    // DIÁLOGOS
    // ============================
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
                txt = overrideTexts[index];
            else if (!string.IsNullOrWhiteSpace(overrideText))
                txt = overrideText;

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
        float timer = 0f;

        while (timer < showTime)
        {
            if (Input.GetKeyDown(KeyCode.E))
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }
    }

    // ==================================================
    // FREEZE + IDLE BLOQUEADO (ANY STATE SAFE)
    // ==================================================
    private IEnumerator FreezePlayerDuringDialogue(GameObject player)
    {
        PlayerController pCtrl = player.GetComponent<PlayerController>();
        Rigidbody rb = player.GetComponent<Rigidbody>();
        Animator animator = player.GetComponentInChildren<Animator>();

        // =========================
        // BLOQUEAR ANIMATOR
        // =========================
        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Dash");
            animator.ResetTrigger("AttackAxe");
            animator.ResetTrigger("AttackSpear");

            animator.Play(idleStateName, 0, 0f);
            animator.Update(0f); // FORÇA APLICAR IDLE
            animator.speed = 0f; // CONGELA NO IDLE
        }

        // =========================
        // BLOQUEAR MOVIMENTO
        // =========================
        if (pCtrl != null)
            pCtrl.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Espera o diálogo acabar
        while (isShowing)
            yield return null;

        // =========================
        // RESTAURAR
        // =========================
        if (animator != null)
            animator.speed = 1f;

        if (pCtrl != null)
            pCtrl.enabled = true;

        if (rb != null)
            rb.isKinematic = false;
    }
}
