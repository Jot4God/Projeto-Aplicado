using UnityEngine;
using System.Collections.Generic;   // <-- para Dictionary
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Portal3D : MonoBehaviour
{
    [Header("Ligação")]
    public Portal3D target;              // Outro portal (destino)
    public Transform exitPoint;          // Onde QUEM VEM DO OUTRO aparece neste portal

    [Header("Opções")]
    public float reentryCooldown = 0.30f;    // evitar reentrada imediata
    public bool requirePlayerTag = true;     // só teleporta objetos com Tag "Player"
    public bool matchExitRotation = true;    // alinhar rotação ao sair
    public bool preserveVelocity = true;     // manter velocidade (reorientada)
    public float exitForwardBoost = 0f;      // boost extra na direção do ExitPoint

    [Header("Entradas Necessárias")]
    [Tooltip("Número de vezes que o objeto tem de entrar no portal até o teleporte ativar. 1 = teleporta logo à primeira.")]
    public int requiredPasses = 1;

    [Header("Câmara (salto instantâneo)")]
    public bool snapMainCamera = true;       // faz a câmara “saltar” sem mostrar a viagem
    public bool alignCameraWithExit = false; // opcional: alinhar também a rotação da câmara com o Exit
        // ---------------------------
    // FREEZE PLAYER
    // ---------------------------
    [Header("Freeze Player")]
    [Tooltip("Tempo que o jogador fica congelado ao entrar no portal.")]
    public float freezePlayerDuration = 2f;
    // ---------------------------
    // NOVO — FADE SCREEN
    // ---------------------------
    [Header("Fade")]
    public GameObject fadeScreen;        // tela preta
    public float fadeDuration = 1f;      // tempo que fica ativa
    // ---------------------------

    // ---------------------------
    // NOVO — APENAS UMA VEZ
    // ---------------------------
    [Header("Uso Único")]
    public bool singleUse = false;       // se true, portal só funciona 1x
    private bool alreadyUsed = false;    // interno
    // ---------------------------

    private Collider trigger;

    // Guarda quantas vezes cada Traveler já passou neste portal
    private Dictionary<PortalTraveler, int> passCounts = new Dictionary<PortalTraveler, int>();

    void Reset()
    {
        trigger = GetComponent<Collider>();
        if (trigger) trigger.isTrigger = true;
    }

    void Awake()
    {
        trigger = GetComponent<Collider>();
        if (trigger && !trigger.isTrigger) trigger.isTrigger = true;

        if (requiredPasses < 1)
            requiredPasses = 1;

        // Garante que a tela preta começa desativada
        if (fadeScreen != null)
            fadeScreen.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        // Se já foi usado e está marcado como "single use", não faz mais nada
        if (singleUse && alreadyUsed)
            return;

        if (!target || !target.exitPoint)
        {
            Debug.LogWarning($"[Portal3D] {name}: Target/Target.exitPoint não definidos.");
            return;
        }

        if (requirePlayerTag && !other.CompareTag("Player"))
            return;

        var traveler = other.GetComponent<PortalTraveler>();

        // cooldown
        if (traveler && traveler.lockedUntil > Time.time)
            return;

        // ---- LÓGICA DAS N VEZES QUE PASSA ----
        if (requiredPasses > 1 && traveler != null)
        {
            int count = 0;
            passCounts.TryGetValue(traveler, out count);
            count++;
            passCounts[traveler] = count;

            if (count < requiredPasses)
            {
                Debug.Log($"[Portal3D] {name}: {other.name} passou {count}x, precisa de {requiredPasses}x para ativar.");
                return;
            }

            passCounts[traveler] = requiredPasses;
        }

        // ------------------------------------
        // ATIVAR FADE SCREEN (se existir)
        // ------------------------------------
        if (fadeScreen != null)
            StartCoroutine(ShowFadeScreen());

        Teleport(other.gameObject, traveler);

        // Marca como usado e desativa, se o portal for single-use
        if (singleUse)
        {
            alreadyUsed = true;
            this.enabled = false; // desativa TODO o script
        }
    }
 IEnumerator FreezePlayer(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        CharacterController cc = obj.GetComponent<CharacterController>();

        MonoBehaviour movementScript = obj.GetComponent<MonoBehaviour>(); 
        // Se me disseres qual é o script de movimento, eu congelo direitinho.

        float timer = freezePlayerDuration;

        // Congelar Rigidbody
        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Congelar CharacterController
        if (cc)
            cc.enabled = false;

        // Espera
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        // Soltar Rigidbody
        if (rb)
            rb.isKinematic = false;

        // Soltar CC
        if (cc)
            cc.enabled = true;
    }
    // --------------------------
    // ROTINA DO FADE
    // --------------------------
    System.Collections.IEnumerator ShowFadeScreen()
    {
        fadeScreen.SetActive(true);
        yield return new WaitForSeconds(fadeDuration);
        fadeScreen.SetActive(false);
    }
    // --------------------------

    void Teleport(GameObject obj, PortalTraveler traveler)
    {
        Transform dest = target.exitPoint;
        var rb = obj.GetComponent<Rigidbody>();
        var cc = obj.GetComponent<CharacterController>();

        Vector3 oldPos = obj.transform.position;
        Quaternion oldRot = obj.transform.rotation;

        Vector3 vWorld = Vector3.zero;
        if (rb && preserveVelocity)
            vWorld = rb.linearVelocity;

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

        if (rb)
        {
            if (preserveVelocity)
            {
                Vector3 vLocalToEntrance = transform.InverseTransformDirection(vWorld);
                Vector3 vToExit = dest.TransformDirection(vLocalToEntrance);
                rb.linearVelocity = vToExit;
            }

            if (exitForwardBoost > 0f)
                rb.linearVelocity += dest.forward * exitForwardBoost;
        }

        if (snapMainCamera && Camera.main != null)
        {
            Transform cam = Camera.main.transform;
            Vector3 delta = obj.transform.position - oldPos;
            cam.position += delta;

            if (alignCameraWithExit)
                cam.rotation = matchExitRotation ? dest.rotation : cam.rotation;
        }

        if (traveler != null)
            traveler.lockedUntil = Time.time + target.reentryCooldown;

        Debug.Log($"[Portal3D] {name} -> {target.name} | {obj.name} @ {dest.position}");
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
