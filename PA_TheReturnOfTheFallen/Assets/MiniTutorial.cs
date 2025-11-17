using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class TutorialStep
{
    public string text;           // Texto a mostrar
    public KeyCode key;           // Tecla que avança, use KeyCode.None se for outro input
    public bool mouse0;           // Avança com Mouse1
    public bool mouse1;           // Avança com Mouse2
    public bool movement;         // Avança com WASD/Setas
}

public class MiniTutorial : MonoBehaviour
{
    [Header("UI")]
    public GameObject tutorialPanel;
    public TMP_Text tutorialText;
    public float fadeTime = 0.5f;

    [Header("Steps")]
    public TutorialStep[] steps;

    private int currentStep = 0;
    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = tutorialPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = tutorialPanel.AddComponent<CanvasGroup>();
        }

        currentStep = 0;
        tutorialPanel.SetActive(true);
        canvasGroup.alpha = 0;
        StartCoroutine(FadeInPanel());
        ShowStep(currentStep);
    }

    void Update()
    {
        if (currentStep >= steps.Length) return;

        TutorialStep step = steps[currentStep];

        if (step.movement && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
            NextStep();
        else if (step.mouse0 && Input.GetMouseButtonDown(0))
            NextStep();
        else if (step.mouse1 && Input.GetMouseButtonDown(1))
            NextStep();
        else if (step.key != KeyCode.None && Input.GetKeyDown(step.key))
            NextStep();
    }

    void ShowStep(int index)
    {
        if (index < steps.Length)
        {
            tutorialText.text = steps[index].text;
        }
        else
        {
            StartCoroutine(FadeOutPanel());
            Debug.Log("Tutorial completo!");
        }
    }

    void NextStep()
    {
        currentStep++;
        ShowStep(currentStep);
    }

    IEnumerator FadeInPanel()
    {
        float t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeTime);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    IEnumerator FadeOutPanel()
    {
        float t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeTime);
            yield return null;
        }
        canvasGroup.alpha = 0;
        tutorialPanel.SetActive(false);
    }
}
