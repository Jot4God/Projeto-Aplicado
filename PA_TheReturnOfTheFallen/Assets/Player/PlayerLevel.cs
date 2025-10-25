using System;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    [Header("Limites")]
    [SerializeField] private int maxLevel = 50;
    [SerializeField] private int startLevel = 1;

    [Header("Curva de XP por nível (XP necessário para passar de N para N+1)")]
    // Podes ajustar no Inspector. Ex.: nível 1 precisa ~100 XP; nível 50 ~5000 XP.
    [SerializeField] private AnimationCurve xpPerLevelCurve = new AnimationCurve(
        new Keyframe(1, 100),
        new Keyframe(50, 5000)
    );

    [Header("Estado Atual")]
    [SerializeField] private int currentLevel;
    [SerializeField] private int xpInCurrentLevel; // XP acumulado no nível atual
    [SerializeField] private int skillPoints;      // Pontos ganhos ao subir de nível

    // Eventos para UI/efeitos
    public event Action<int> OnLevelUp;                 // novo nível
    public event Action<int, int, int> OnXPChanged;     // (xpAtualNoNivel, xpNecessário, nível)
    public event Action<int> OnSkillPointsChanged;      // pontos atuais

    // Propriedades públicas
    public int CurrentLevel => currentLevel;
    public int SkillPoints => skillPoints;
    public int XPInCurrentLevel => xpInCurrentLevel;
    public int XPRequiredThisLevel => GetXPRequiredForLevel(currentLevel);

    private void Awake()
    {
        if (currentLevel <= 0) currentLevel = Mathf.Clamp(startLevel, 1, maxLevel);
        xpInCurrentLevel = Mathf.Max(0, xpInCurrentLevel);
        skillPoints = Mathf.Max(0, skillPoints);
        RaiseXPEvent();
        OnSkillPointsChanged?.Invoke(skillPoints);
    }

    /// <summary>
    /// Dá XP ao jogador. Faz múltiplos level-ups se for preciso.
    /// </summary>
    public void AddXP(int amount)
    {
        if (amount <= 0 || currentLevel >= maxLevel) return;

        xpInCurrentLevel += amount;

        // Loop de subida de nível enquanto houver XP suficiente
        while (currentLevel < maxLevel && xpInCurrentLevel >= GetXPRequiredForLevel(currentLevel))
        {
            xpInCurrentLevel -= GetXPRequiredForLevel(currentLevel);
            currentLevel++;

            // Dá 1 ponto por nível. Muda aqui se quiseres outra regra.
            skillPoints++;
            OnLevelUp?.Invoke(currentLevel);
            OnSkillPointsChanged?.Invoke(skillPoints);
        }

        // Se atingiu maxLevel, trava XP a 0..req-1 ou zera, como preferires:
        if (currentLevel >= maxLevel)
        {
            // Mantém XP “cheio” visualmente:
            xpInCurrentLevel = Mathf.Min(xpInCurrentLevel, GetXPRequiredForLevel(maxLevel) - 1);
        }

        RaiseXPEvent();
    }

    /// <summary>
    /// Consome pontos (para o que quiseres usar no teu jogo).
    /// </summary>
    public bool TryConsumePoints(int cost)
    {
        if (cost <= 0 || skillPoints < cost) return false;
        skillPoints -= cost;
        OnSkillPointsChanged?.Invoke(skillPoints);
        return true;
    }

    /// <summary>
    /// Reembolsa pontos (se fizeres respec, etc.).
    /// </summary>
    public void RefundPoints(int amount)
    {
        if (amount <= 0) return;
        skillPoints += amount;
        OnSkillPointsChanged?.Invoke(skillPoints);
    }

    /// <summary>
    /// XP necessário para passar do 'level' para 'level+1'.
    /// </summary>
    private int GetXPRequiredForLevel(int level)
    {
        level = Mathf.Clamp(level, 1, maxLevel);
        // Garante pelo menos 1 para evitar divisão por zero/loops
        return Mathf.Max(1, Mathf.RoundToInt(xpPerLevelCurve.Evaluate(level)));
    }

    private void RaiseXPEvent()
    {
        OnXPChanged?.Invoke(xpInCurrentLevel, XPRequiredThisLevel, currentLevel);
    }

    // ==== Utilidade (Save/Load simples se quiseres persistir) ====

    [Serializable]
    public struct LevelState
    {
        public int level;
        public int xpInLevel;
        public int points;
    }

    public LevelState GetState() => new LevelState
    {
        level = currentLevel,
        xpInLevel = xpInCurrentLevel,
        points = skillPoints
    };

    public void SetState(LevelState s)
    {
        currentLevel = Mathf.Clamp(s.level, 1, maxLevel);
        xpInCurrentLevel = Mathf.Max(0, s.xpInLevel);
        skillPoints = Mathf.Max(0, s.points);
        RaiseXPEvent();
        OnSkillPointsChanged?.Invoke(skillPoints);
        OnLevelUp?.Invoke(currentLevel); // dispara para sincronizar UI, se precisares
    }
}
