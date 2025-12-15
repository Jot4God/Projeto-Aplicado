using UnityEngine;

public class InteractionIcon : MonoBehaviour
{
    [Header("Referência Visual")]
    public GameObject iconChild;   // ponto de exclamação (filho)

    [Header("Portal que desbloqueia")]
    public Portal3D requiredPortal;

    private bool unlocked = false;

    private void Awake()
    {
        if (iconChild != null)
            iconChild.SetActive(false);
    }

    private void OnEnable()
    {
        Portal3D.OnPortalUsed += HandlePortalUsed;
    }

    private void OnDisable()
    {
        Portal3D.OnPortalUsed -= HandlePortalUsed;
    }

    private void HandlePortalUsed(Portal3D portal)
    {
        if (unlocked)
            return;

        if (portal == requiredPortal)
        {
            unlocked = true;
            iconChild.SetActive(true);
        }
    }
}
