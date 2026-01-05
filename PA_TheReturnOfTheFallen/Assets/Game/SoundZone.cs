using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SoundZone3D : MonoBehaviour
{
    [Header("Quem ativa a zona")]
    public string playerTag = "Player";

    [Header("AudioSources controlados")]
    [Tooltip("Se vazio, o script vai procurar AudioSources nos filhos automaticamente.")]
    public List<AudioSource> sources = new();

    [Header("Comportamento")]
    [Range(0.05f, 5f)] public float fadeTime = 1.0f;
    [Range(0f, 2f)] public float targetVolumeMultiplier = 1.0f;

    [Tooltip("Se true, dá Play nos AudioSources quando o player entra na zona (se não estiverem a tocar).")]
    public bool playOnEnter = true;

    [Tooltip("Se true, faz Stop no AudioSource quando o fade-out termina (ao sair).")]
    public bool stopOnExit = true;

    [Tooltip("Se a lista 'sources' estiver vazia, procura AudioSources nos filhos. Inclui filhos desativados?")]
    public bool includeInactiveChildren = false;

    [Header("Inicialização")]
    [Tooltip("Se true, começa com volume a 0 e (opcionalmente) para os sons, para garantir silêncio antes de entrar.")]
    public bool startSilent = true;

    // Guardar volumes “originais” para restaurar quando entrar
    private readonly Dictionary<AudioSource, float> _baseVolumes = new();
    private readonly Dictionary<AudioSource, Coroutine> _running = new();

    // Suporta múltiplos colliders do player (ex.: capsule + trigger dos pés)
    private int _insideCount = 0;

    private void Awake()
    {
        CacheSourcesIfNeeded();
        CacheBaseVolumes();

        if (startSilent)
        {
            SetAllVolumesInstant(0f);

            if (stopOnExit)
                StopAllSources();
        }
    }

    private void CacheSourcesIfNeeded()
    {
        if (sources != null && sources.Count > 0) return;

        var found = GetComponentsInChildren<AudioSource>(includeInactiveChildren);
        sources = new List<AudioSource>(found);
    }

    private void CacheBaseVolumes()
    {
        _baseVolumes.Clear();
        foreach (var src in sources)
        {
            if (!src) continue;
            _baseVolumes[src] = Mathf.Clamp01(src.volume);
        }
    }

    private void SetAllVolumesInstant(float v)
    {
        foreach (var src in sources)
        {
            if (!src) continue;
            src.volume = Mathf.Clamp01(v);
        }
    }

    private void StopAllSources()
    {
        foreach (var src in sources)
        {
            if (!src) continue;
            src.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        _insideCount++;
        if (_insideCount == 1) EnterZone();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        _insideCount = Mathf.Max(0, _insideCount - 1);
        if (_insideCount == 0) ExitZone();
    }

    private void EnterZone()
    {
        foreach (var src in sources)
        {
            if (!src) continue;

            if (playOnEnter && !src.isPlaying)
                src.Play();

            float baseVol = _baseVolumes.TryGetValue(src, out var v) ? v : 1f;
            float target = Mathf.Clamp01(baseVol * targetVolumeMultiplier);

            StartFade(src, target, fadeTime, stopAfter: false);
        }
    }

    private void ExitZone()
    {
        foreach (var src in sources)
        {
            if (!src) continue;

            StartFade(src, 0f, fadeTime, stopAfter: stopOnExit);
        }
    }

    private void StartFade(AudioSource src, float targetVolume, float time, bool stopAfter)
    {
        if (_running.TryGetValue(src, out var c) && c != null)
            StopCoroutine(c);

        _running[src] = StartCoroutine(FadeRoutine(src, targetVolume, time, stopAfter));
    }

    private IEnumerator FadeRoutine(AudioSource src, float target, float time, bool stopAfter)
    {
        float start = src.volume;

        if (time <= 0f)
        {
            src.volume = target;
            if (stopAfter && Mathf.Approximately(target, 0f)) src.Stop();
            yield break;
        }

        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / time);
            src.volume = Mathf.Lerp(start, target, k);
            yield return null;
        }

        src.volume = target;
        if (stopAfter && Mathf.Approximately(target, 0f))
            src.Stop();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        fadeTime = Mathf.Max(0.01f, fadeTime);
        targetVolumeMultiplier = Mathf.Max(0f, targetVolumeMultiplier);
    }
#endif
}
