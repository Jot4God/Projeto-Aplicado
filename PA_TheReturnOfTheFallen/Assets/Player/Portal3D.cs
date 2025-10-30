using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Portal3D : MonoBehaviour
{
    [Header("Ligação")]
    public Portal3D target;        // Outro portal (destino)
    public Transform exitPoint;    // Onde QUEM VEM DO OUTRO aparece neste portal

    [Header("Opções")]
    public float reentryCooldown = 0.30f;   // evitar reentrada imediata
    public bool requirePlayerTag = true;    // só teleporta objetos com Tag "Player"
    public bool matchExitRotation = true;   // alinhar rotação ao sair
    public bool preserveVelocity = true;    // manter velocidade (reorientada)
    public float exitForwardBoost = 0f;     // boost extra na direção do ExitPoint

    private Collider trigger;

    void Reset()
    {
        trigger = GetComponent<Collider>();
        if (trigger) trigger.isTrigger = true;
    }

    void Awake()
    {
        trigger = GetComponent<Collider>();
        if (trigger && !trigger.isTrigger) trigger.isTrigger = true;
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
        if (traveler && traveler.lockedUntil > Time.time)
            return;

        Teleport(other.gameObject, traveler);
    }

    void Teleport(GameObject obj, PortalTraveler traveler)
    {
        Transform dest = target.exitPoint;
        var rb = obj.GetComponent<Rigidbody>();
        var cc = obj.GetComponent<CharacterController>(); // se existir

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

        // 5) Cooldown para evitar ping-pong
        if (traveler != null)
            traveler.lockedUntil = Time.time + target.reentryCooldown;

        // Debug útil
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
