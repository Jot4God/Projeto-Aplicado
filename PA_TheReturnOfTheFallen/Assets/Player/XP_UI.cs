using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class XP_UI : MonoBehaviour
{
    [Header("Referências")]
    public PlayerLevel playerLevel;  // Arrasta o Player aqui
    public Slider xpBar;             // Arrasta o Slider XPBar aqui
    public Text levelText;           // (Opcional) Mostra o nível atual

    private void Awake()
    {
        if (playerLevel == null)
        playerLevel = Object.FindFirstObjectByType<PlayerLevel>();
    }

    private void OnEnable()
    {
        if (playerLevel != null)
        {
            playerLevel.OnXPChanged += UpdateXPBar;
            playerLevel.OnLevelUp += UpdateLevelText;
        }
    }

    private void OnDisable()
    {
        if (playerLevel != null)
        {
            playerLevel.OnXPChanged -= UpdateXPBar;
            playerLevel.OnLevelUp -= UpdateLevelText;
        }
    }

    private void Start()
    {
        if (playerLevel != null)
        {
            UpdateXPBar(playerLevel.XPInCurrentLevel, playerLevel.XPRequiredThisLevel, playerLevel.CurrentLevel);
            UpdateLevelText(playerLevel.CurrentLevel);
        }
    }

    private void UpdateXPBar(int xpAtual, int xpNecessario, int nivel)
    {
        if (xpBar == null) return;
        xpBar.maxValue = xpNecessario;

        StopAllCoroutines();
        StartCoroutine(SmoothFill(xpAtual));
    }

    private IEnumerator SmoothFill(float target)
    {
        float start = xpBar.value;
        float t = 0f;
        while (t < 0.25f)
        {
            t += Time.deltaTime;
            xpBar.value = Mathf.Lerp(start, target, t / 0.25f);
            yield return null;
        }
        xpBar.value = target;
    }

    private void UpdateLevelText(int nivel)
    {
        if (levelText != null)
            levelText.text = "Lv. " + nivel.ToString();
    }
}
