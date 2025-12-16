using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PortalUnlockWall : MonoBehaviour
{
    [Header("Portal que desbloqueia esta parede")]
    public Portal3D unlockPortal;

    [Header("Comportamento")]
    public bool disableOnlyCollider = true;
    public bool disableWholeObject = false;

    private BoxCollider box;
    private bool unlocked = false;

    void Awake()
    {
        box = GetComponent<BoxCollider>();
    }

    void OnEnable()
    {
        Portal3D.OnPortalUsed += OnPortalUsed;
    }

    void OnDisable()
    {
        Portal3D.OnPortalUsed -= OnPortalUsed;
    }

    private void OnPortalUsed(Portal3D portal)
    {
        if (unlocked) return;
        if (portal != unlockPortal) return;

        Unlock();
    }

    private void Unlock()
    {
        unlocked = true;

        if (disableOnlyCollider && box)
            box.enabled = false;

        if (disableWholeObject)
            gameObject.SetActive(false);
    }
}
