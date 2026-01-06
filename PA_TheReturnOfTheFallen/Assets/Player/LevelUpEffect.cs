using UnityEngine;
using TMPro;
using System.Collections;

public class LevelUpEffect : MonoBehaviour
{
    [Header("Configurações")]
    public PlayerLevel playerLevel;
    public TMP_Text textoLevel;
    public CanvasGroup canvasGroup;

    [Header("Animação - Tempos")]
    public float tempoPop = 0.3f;        // Tempo da explosão inicial (rápido)
    public float tempoAssentar = 0.2f;   // Tempo para voltar ao tamanho normal
    public float tempoEspera = 1.0f;     // Tempo parado no ecrã
    public float tempoDesaparecer = 0.8f;// Tempo do fade out

    [Header("Animação - Tamanhos e Movimento")]
    public float escalaMaximaPop = 1.5f; // O tamanho máximo que atinge na explosão
    public float escalaFinal = 1.2f;     // O tamanho em que fica parado
    public float anguloInclinacao = -5f; // Ligeira rotação ao aparecer
    public float distanciaSubida = 50f;  // Quanto sobe ao desaparecer

    private Vector3 posicaoOriginal;

    private void Start()
    {
        if (canvasGroup != null) canvasGroup.alpha = 0;

        // Guarda a posição onde puseste o texto no editor para saber onde voltar
        posicaoOriginal = transform.localPosition;
        
        if (playerLevel == null) 
            playerLevel = Object.FindFirstObjectByType<PlayerLevel>();

        if (playerLevel != null)
        {
            playerLevel.OnLevelUp += MostrarAnimacao;
        }
    }

    private void OnDestroy()
    {
        if (playerLevel != null)
        {
            playerLevel.OnLevelUp -= MostrarAnimacao;
        }
    }

    void MostrarAnimacao(int novoNivel)
    {
        if (textoLevel != null) 
            textoLevel.text = "LEVEL " + novoNivel + "!";

        StopAllCoroutines();
        StartCoroutine(AnimarComEstilo());
    }

    IEnumerator AnimarComEstilo()
    {
        // --- RESET INICIAL ---
        transform.localScale = Vector3.zero;
        transform.localPosition = posicaoOriginal;
        transform.localRotation = Quaternion.identity; // Rotação a zero
        canvasGroup.alpha = 1;

        // --- FASE 1: O POP ELÁSTICO (Cresce muito e inclina) ---
        float t = 0;
        while (t < tempoPop)
        {
            t += Time.deltaTime;
            float progress = t / tempoPop;
            // Mathf.SmoothStep faz o movimento começar e acabar mais suave
            float currentProgress = Mathf.SmoothStep(0, 1, progress);

            // Cresce até à escalaMaximaPop (ex: 1.5x)
            float escala = Mathf.Lerp(0, escalaMaximaPop, currentProgress);
            transform.localScale = new Vector3(escala, escala, 1);

            // Inclina um bocadinho (ex: -5 graus)
            float angulo = Mathf.Lerp(0, anguloInclinacao, currentProgress);
            transform.localRotation = Quaternion.Euler(0, 0, angulo);

            yield return null;
        }

        // --- FASE 2: ASSENTAR (Encolhe para o tamanho final e endireita) ---
        t = 0;
        while (t < tempoAssentar)
        {
            t += Time.deltaTime;
            float progress = t / tempoAssentar;
            // SmoothStep de novo para suavizar
            float currentProgress = Mathf.SmoothStep(0, 1, progress);

            // Encolhe de 1.5x para 1.2x (dá o efeito de ressalto)
            float escala = Mathf.Lerp(escalaMaximaPop, escalaFinal, currentProgress);
            transform.localScale = new Vector3(escala, escala, 1);

            // Roda de volta a zero graus
            transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, anguloInclinacao), Quaternion.identity, currentProgress);

            yield return null;
        }
        // Garante que fica exatamente no tamanho e rotação final
        transform.localScale = new Vector3(escalaFinal, escalaFinal, 1);
        transform.localRotation = Quaternion.identity;


        // --- FASE 3: ESPERAR ---
        yield return new WaitForSeconds(tempoEspera);


        // --- FASE 4: DESAPARECER A SUBIR ---
        t = 0;
        Vector3 posicaoInicialSubida = transform.localPosition;
        // Define o ponto para onde vai subir (50 pixeis para cima)
        Vector3 posicaoFinalSubida = posicaoOriginal + new Vector3(0, distanciaSubida, 0);

        while (t < tempoDesaparecer)
        {
            t += Time.deltaTime;
            float progress = t / tempoDesaparecer;

            // Fade Out (Alpha de 1 para 0)
            canvasGroup.alpha = Mathf.Lerp(1, 0, progress);

            // Mover para cima suavemente
            transform.localPosition = Vector3.Lerp(posicaoInicialSubida, posicaoFinalSubida, progress);

            yield return null;
        }
        
        canvasGroup.alpha = 0;
        // Reseta a posição para a próxima vez
        transform.localPosition = posicaoOriginal; 
    }
}