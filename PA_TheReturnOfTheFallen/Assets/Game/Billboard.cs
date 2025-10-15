using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        Vector3 lookDir = transform.position - cam.transform.position;
        lookDir.y = 0; // trava o eixo vertical
        transform.rotation = Quaternion.LookRotation(lookDir);
    }
}
