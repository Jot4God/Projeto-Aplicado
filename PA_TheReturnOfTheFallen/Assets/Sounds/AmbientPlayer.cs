using UnityEngine;
using System.Collections;

public class AmbientPlayer : MonoBehaviour
{
    public static AmbientPlayer Instance { get; private set; }

    [Header("Source")]
    [SerializeField] private AudioSource source;

    [Header("Start Ambient")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private AudioClip startClip;
    [SerializeField, Range(0f, 1f)] private float startVolume = 1f;

    [Header("Fade")]
    [SerializeField] private float fadeTime = 0.8f;

    private Coroutine routine;

    // ✅ Expor o que está a tocar agora (para triggers guardarem e restaurarem)
    public AudioClip CurrentClip => source != null ? source.clip : null;
    public float CurrentVolume => source != null ? source.volume : 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (source == null) source = GetComponent<AudioSource>();
        source.loop = true;
        source.playOnAwake = false;
    }

    void Start()
    {
        if (playOnStart && startClip != null)
        {
            // Começa a tocar logo (sem risco de travar, mesmo com timeScale=0)
            source.Stop();
            source.clip = startClip;
            source.volume = startVolume;
            source.Play();
        }
    }

    public void PlayAmbient(AudioClip clip, float targetVolume = 1f)
    {
        if (clip == null || source == null) return;
        if (source.clip == clip && source.isPlaying) return;

        if (routine != null) StopCoroutine(routine);

        // Se fadeTime <= 0, troca instantâneo
        if (fadeTime <= 0f)
        {
            source.Stop();
            source.clip = clip;
            source.volume = targetVolume;
            source.Play();
            return;
        }

        routine = StartCoroutine(FadeSwap(clip, targetVolume));
    }

    private IEnumerator FadeSwap(AudioClip newClip, float targetVolume)
    {
        float startVol = source.isPlaying ? source.volume : 0f;

        // Fade out (usa unscaledDeltaTime para não depender do Time.timeScale)
        for (float t = 0f; t < fadeTime; t += Time.unscaledDeltaTime)
        {
            source.volume = Mathf.Lerp(startVol, 0f, t / fadeTime);
            yield return null;
        }
        source.volume = 0f;

        source.Stop();
        source.clip = newClip;
        source.Play();

        // Fade in
        for (float t = 0f; t < fadeTime; t += Time.unscaledDeltaTime)
        {
            source.volume = Mathf.Lerp(0f, targetVolume, t / fadeTime);
            yield return null;
        }
        source.volume = targetVolume;

        routine = null;
    }
}
