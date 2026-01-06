using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AmbientZoneTrigger3D : MonoBehaviour
{
    [Header("Ambient a ativar nesta zona")]
    public AudioClip ambientClip;
    [Range(0f, 1f)] public float ambientVolume = 1f;

    [Header("Regras")]
    public bool requirePlayerTag = true;

    [Header("Ao sair da zona")]
    [Tooltip("Se ligado, ao sair volta ao ambiente anterior (clip + volume).")]
    public bool restorePreviousOnExit = true;

    [Tooltip("Se ligado, só troca o ambiente uma vez (primeira entrada).")]
    public bool singleUse = false;

    private bool used = false;

    // Guardar ambiente anterior para restaurar
    private AudioClip previousClip;
    private float previousVolume;

    // Evita triggers repetidos se o Player tiver vários colliders
    private int insideCount = 0;

    void Awake()
    {
        var col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
            col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (singleUse && used) return;
        if (requirePlayerTag && !other.CompareTag("Player")) return;

        insideCount++;
        if (insideCount != 1) return; // só na primeira entrada real

        if (AmbientPlayer.Instance == null)
        {
            Debug.LogWarning("[AmbientZoneTrigger3D] Não existe AmbientPlayer na cena.");
            return;
        }

        // Guardar o que estava a tocar antes, para restaurar ao sair
        if (restorePreviousOnExit)
        {
            previousClip = AmbientPlayer.Instance.CurrentClip;
            previousVolume = AmbientPlayer.Instance.CurrentVolume;
        }

        if (ambientClip != null)
            AmbientPlayer.Instance.PlayAmbient(ambientClip, ambientVolume);

        used = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (requirePlayerTag && !other.CompareTag("Player")) return;

        insideCount = Mathf.Max(insideCount - 1, 0);
        if (insideCount != 0) return; // só quando saiu “mesmo”

        if (!restorePreviousOnExit) return;

        if (AmbientPlayer.Instance == null) return;

        // Se não havia nada antes, não faz nada
        if (previousClip != null)
            AmbientPlayer.Instance.PlayAmbient(previousClip, previousVolume);
    }
}
