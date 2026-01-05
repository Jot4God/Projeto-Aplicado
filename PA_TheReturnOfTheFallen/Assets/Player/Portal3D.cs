using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class Portal3D : MonoBehaviour
{
    // 🔔 EVENTO GLOBAL
    public static event Action<Portal3D> OnPortalUsed;

    [Header("Ligação")]
    public Portal3D target;
    public Transform exitPoint;

    [Header("Opções")]
    public float reentryCooldown = 0.30f;
    public bool requirePlayerTag = true;
    public bool matchExitRotation = true;
    public bool preserveVelocity = true;
    public float exitForwardBoost = 0f;

    [Header("Entradas Necessárias")]
    public int requiredPasses = 1;

    [Header("Câmara")]
    public bool snapMainCamera = true;
    public bool alignCameraWithExit = false;

    // ===========================
    // FREEZE PLAYER
    // ===========================
    [Header("Freeze Player")]
    public float freezePlayerDuration = 2f;

    // ===========================
    // FADE SCREEN
    // ===========================
    [Header("Fade")]
    public GameObject fadeScreen;
    public float fadeDuration = 1f;

    // ===========================
    // AMBIENTE (NOVO - RECOMENDADO)
    // ===========================
    [Header("Ambiente (Recomendado)")]
    [Tooltip("Clip de ambiente que este portal deve ativar.")]
    public AudioClip ambientClip;

    [Range(0f, 1f)]
    public float ambientVolume = 1f;

    [Tooltip("Se desligado, este portal não altera o ambiente.")]
    public bool changeAmbientOnUse = true;

    // ===========================
    // LEGACY (OPCIONAL)
    // ===========================
    [Header("Música Ambiente (Legacy - evita tocar aqui)")]
    [Tooltip("Opcional: se não quiseres arrastar o AudioClip, podes apontar para um objeto com AudioSource e ele usa o clip desse AudioSource. Este AudioSource NÃO deve tocar por si.")]
    public GameObject ambientMusicObject;

    [Header("Uso Único")]
    public bool singleUse = false;
    private bool alreadyUsed = false;

    private bool isInUse = false;
    private Collider trigger;
    private Dictionary<PortalTraveler, int> passCounts = new Dictionary<PortalTraveler, int>();

    void Awake()
    {
        trigger = GetComponent<Collider>();
        if (trigger && !trigger.isTrigger)
            trigger.isTrigger = true;

        if (fadeScreen != null)
            fadeScreen.SetActive(false);

        // Se tiveres "legacy object" com AudioSource, garante que não toca sozinho
        if (ambientMusicObject != null)
        {
            var src = ambientMusicObject.GetComponent<AudioSource>();
            if (src != null)
            {
                src.playOnAwake = false;
                if (src.isPlaying) src.Stop();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isInUse)
            return;

        if (singleUse && alreadyUsed)
            return;

        if (!target || !target.exitPoint)
            return;

        if (requirePlayerTag && !other.CompareTag("Player"))
            return;

        var traveler = other.GetComponent<PortalTraveler>();
        if (traveler && traveler.lockedUntil > Time.time)
            return;

        if (requiredPasses > 1 && traveler != null)
        {
            passCounts.TryGetValue(traveler, out int count);
            count++;
            passCounts[traveler] = count;

            if (count < requiredPasses)
                return;
        }

        StartCoroutine(PortalSequence(other.gameObject, traveler));
    }

    // ===========================
    // SEQUÊNCIA COMPLETA
    // ===========================
    IEnumerator PortalSequence(GameObject obj, PortalTraveler traveler)
    {
        isInUse = true;

        // 1️⃣ FREEZE
        StartCoroutine(FreezePlayer(obj));

        // 2️⃣ FADE IN
        if (fadeScreen != null)
            fadeScreen.SetActive(true);

        yield return null;

        // 3️⃣ AMBIENTE (AGORA TROCA NO AmbientPlayer GLOBAL)
        HandleAmbientMusic();

        // 4️⃣ TELEPORTE
        Teleport(obj, traveler);

        // ✅ DISPARA EVENTO (PORTAL FOI USADO)
        OnPortalUsed?.Invoke(this);

        // 5️⃣ ESPERA FADE
        yield return new WaitForSeconds(fadeDuration);

        // 6️⃣ FADE OUT
        if (fadeScreen != null)
            fadeScreen.SetActive(false);

        if (singleUse)
        {
            alreadyUsed = true;
            enabled = false;
        }

        isInUse = false;
    }

    // ===========================
    // FREEZE PLAYER
    // ===========================
    IEnumerator FreezePlayer(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        CharacterController cc = obj.GetComponent<CharacterController>();

        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (cc)
            cc.enabled = false;

        yield return new WaitForSeconds(freezePlayerDuration);

        if (rb)
            rb.isKinematic = false;

        if (cc)
            cc.enabled = true;
    }

    // ===========================
    // AMBIENTE (SÓ 1 A TOCAR)
    // ===========================
    void HandleAmbientMusic()
    {
        if (!changeAmbientOnUse)
            return;

        AudioClip clipToPlay = ambientClip;
        float volToPlay = ambientVolume;

        // Fallback legacy: se não arrastares o clip, vai buscar ao AudioSource do objeto
        if (clipToPlay == null && ambientMusicObject != null)
        {
            AudioSource legacySource = ambientMusicObject.GetComponent<AudioSource>();
            if (legacySource != null)
            {
                // garante que não toca por si
                if (legacySource.isPlaying) legacySource.Stop();
                legacySource.playOnAwake = false;

                clipToPlay = legacySource.clip;
                volToPlay = Mathf.Clamp01(legacySource.volume);
            }
        }

        if (clipToPlay == null)
            return;

        // ✅ Toca SEMPRE no AmbientPlayer global (isto pára o anterior)
        if (AmbientPlayer.Instance != null)
        {
            AmbientPlayer.Instance.PlayAmbient(clipToPlay, volToPlay);
        }
        else
        {
            Debug.LogWarning("[Portal3D] Não existe AmbientPlayer na cena. Cria um AmbientPlayer global com AudioSource + script AmbientPlayer.");
        }
    }

    // ===========================
    // TELEPORTE
    // ===========================
    void Teleport(GameObject obj, PortalTraveler traveler)
    {
        Transform dest = target.exitPoint;
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        CharacterController cc = obj.GetComponent<CharacterController>();

        Vector3 oldPos = obj.transform.position;
        Vector3 velocityWorld = Vector3.zero;

        if (rb && preserveVelocity)
            velocityWorld = rb.linearVelocity;

        Quaternion finalRot = matchExitRotation ? dest.rotation : obj.transform.rotation;

        if (cc)
        {
            cc.enabled = false;
            obj.transform.SetPositionAndRotation(dest.position, finalRot);
            cc.enabled = true;
        }
        else
        {
            obj.transform.SetPositionAndRotation(dest.position, finalRot);
        }

        if (rb && preserveVelocity)
        {
            Vector3 localVel = transform.InverseTransformDirection(velocityWorld);
            rb.linearVelocity = dest.TransformDirection(localVel);

            if (exitForwardBoost > 0f)
                rb.linearVelocity += dest.forward * exitForwardBoost;
        }

        if (snapMainCamera && Camera.main != null)
        {
            Camera.main.transform.position += obj.transform.position - oldPos;

            if (alignCameraWithExit)
                Camera.main.transform.rotation = finalRot;
        }

        if (traveler != null)
            traveler.lockedUntil = Time.time + target.reentryCooldown;
    }

    void OnDrawGizmos()
    {
        if (exitPoint)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(exitPoint.position, 0.15f);
            Gizmos.DrawRay(exitPoint.position, exitPoint.forward * 0.6f);
        }
    }
}
