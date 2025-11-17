using UnityEngine;
using System.Collections.Generic;   // <-- para Dictionary

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
    }

    void OnTriggerEnter(Collider other)
    {
        if (!target || !target.exitPoint)
        {
            Debug.LogWarning($"[Portal3D] {name}: Target/Target.exitPoint não definidos.");
            return;
        }

        if (requirePlayerTag && !other.CompareTag("Player"))
            return;

        var traveler = other.GetComponent<PortalTraveler>();

        // Se tiver cooldown ainda a contar, não entra
        if (traveler && traveler.lockedUntil > Time.time)
            return;

        // ---- LÓGICA DAS N VEZES QUE PASSA ----
        if (requiredPasses > 1 && traveler != null)
        {
            int count = 0;
            passCounts.TryGetValue(traveler, out count);
            count++;
            passCounts[traveler] = count;

            // Ainda não atingiu o nº de entradas necessárias → não teleporta
            if (count < requiredPasses)
            {
                Debug.Log($"[Portal3D] {name}: {other.name} passou {count}x, precisa de {requiredPasses}x para ativar.");
                return;
            }

            // Se quiseres que, depois de ativar uma vez, não volte a exigir as N entradas,
            // podes 'fixar' o valor:
            passCounts[traveler] = requiredPasses;
        }
        // Se não houver PortalTraveler mas mesmo assim queres controlar por tag Player,
        // então este objeto vai simplesmente teleportar sempre que entrar.
        // (requiredPasses só funciona por Traveler.)

        Teleport(other.gameObject, traveler);
    }

    void Teleport(GameObject obj, PortalTraveler traveler)
    {
        Transform dest = target.exitPoint;
        var rb = obj.GetComponent<Rigidbody>();
        var cc = obj.GetComponent<CharacterController>(); // se existir

        // --- 0) Guardar estado antes do teleporte ---
        Vector3 oldPos = obj.transform.position;
        Quaternion oldRot = obj.transform.rotation;

        // 1) Capturar velocidade antes de mexer na pose
        Vector3 vWorld = Vector3.zero;
        if (rb && preserveVelocity)
            vWorld = rb.linearVelocity;

        // 2) Calcular rotação final
        Quaternion finalRot = matchExitRotation ? dest.rotation : obj.transform.rotation;

        // 3) Mover de forma segura
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

        // 4) Reorientar velocidade
        if (rb)
        {
            if (preserveVelocity)
            {
                // Converte a velocidade para o espaço local deste portal e re-projeta no espaço do ExitPoint do destino
                Vector3 vLocalToEntrance = transform.InverseTransformDirection(vWorld);
                Vector3 vToExit = dest.TransformDirection(vLocalToEntrance);
                rb.linearVelocity = vToExit;
            }

            if (exitForwardBoost > 0f)
                rb.linearVelocity += dest.forward * exitForwardBoost;
        }

        // 5) “Snapping” da câmara para evitar mostrar a viagem
        if (snapMainCamera && Camera.main != null)
        {
            Transform cam = Camera.main.transform;

            // Delta do teleporte do objeto
            Vector3 delta = obj.transform.position - oldPos;

            // Reposicionar a câmara instantaneamente mantendo o offset relativo
            cam.position += delta;

            if (alignCameraWithExit)
                cam.rotation = matchExitRotation ? dest.rotation : cam.rotation;
        }

        // 6) Cooldown para evitar ping-pong
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
