using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTreeUI : MonoBehaviour
{
    [Header("Referências principais")]
    [SerializeField] private SkillTreeState tree;         // arrasta o SkillTreeState
    [SerializeField] private PlayerLevel playerLevel;     // arrasta o PlayerLevel

    [Header("Botões existentes na cena")]
    [SerializeField] private List<Button> skillButtons = new();    // arrasta manualmente os botões (na mesma ordem das skills)
    [SerializeField] private List<TMP_Text> skillLabels = new();   // arrasta os textos (um por botão)

    [Header("Assets das Skills")]
    [SerializeField] private List<SkillDefinition> skills = new(); // arrasta os 3 assets (Armor, Weapon, Spells)

    [Header("Textos de estado")]
    [SerializeField] private TMP_Text skillPointsText;    // texto que mostra os skill points
    [SerializeField] private TMP_Text levelText;          // texto que mostra o level

    private void Awake()
    {
        if (!tree) tree = FindAnyObjectByType<SkillTreeState>();
        if (!playerLevel) playerLevel = FindAnyObjectByType<PlayerLevel>();
    }

    private void OnEnable()
    {
        HookEvents(true);
        SetupButtons();
        RefreshAll();
    }

    private void OnDisable()
    {
        HookEvents(false);
    }

    private void HookEvents(bool on)
    {
        if (playerLevel == null) return;

        if (on)
        {
            playerLevel.OnSkillPointsChanged += OnSPChanged;
            playerLevel.OnLevelUp += OnLevelUp;
            playerLevel.OnXPChanged += OnXPChanged;
        }
        else
        {
            playerLevel.OnSkillPointsChanged -= OnSPChanged;
            playerLevel.OnLevelUp -= OnLevelUp;
            playerLevel.OnXPChanged -= OnXPChanged;
        }
    }

    private void SetupButtons()
    {
        // certifica-se de que listas têm o mesmo tamanho
        int count = Mathf.Min(skills.Count, skillButtons.Count, skillLabels.Count);

        for (int i = 0; i < count; i++)
        {
            var s = skills[i];
            var btn = skillButtons[i];
            var label = skillLabels[i];

            if (!s || !btn || !label) continue;

            label.text = $"{s.DisplayName}: 0";

            // limpa eventos antigos antes de adicionar
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                if (tree.TryRankUp(s))
                    RefreshOne(i);
                RefreshButtons();
                RefreshSkillPoints();
            });
        }
    }

    private void OnSPChanged(int sp)
    {
        RefreshSkillPoints();
        RefreshButtons();
    }

    private void OnLevelUp(int lvl)
    {
        if (levelText) levelText.text = $"Level: {lvl}";
        RefreshButtons();
    }

    private void OnXPChanged(int currentXP, int requiredXP, int gained)
    {
        RefreshAll();
    }

    private void RefreshAll()
    {
        for (int i = 0; i < skills.Count; i++)
            RefreshOne(i);

        RefreshButtons();
        RefreshSkillPoints();

        if (levelText && playerLevel != null)
            levelText.text = $"Level: {playerLevel.CurrentLevel}";
    }

    private void RefreshOne(int index)
    {
        if (index < 0 || index >= skills.Count) return;

        var s = skills[index];
        if (!s || index >= skillLabels.Count) return;

        var label = skillLabels[index];
        int rank = tree.GetRank(s);
        label.text = $"{s.DisplayName}: {rank}";
    }

    private void RefreshButtons()
    {
        int count = Mathf.Min(skills.Count, skillButtons.Count);
        for (int i = 0; i < count; i++)
        {
            var s = skills[i];
            if (!s) continue;
            var btn = skillButtons[i];
            btn.interactable = tree.CanRankUp(s);
        }
    }

    private void RefreshSkillPoints()
    {
        if (skillPointsText && playerLevel != null)
            skillPointsText.text = $"Skill Points: {playerLevel.SkillPoints}";
    }
}
