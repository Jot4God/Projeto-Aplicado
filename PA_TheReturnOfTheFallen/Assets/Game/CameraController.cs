using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    private Vector3 offset;
    public float smoothSpeed = 5f;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        if (player != null)
            offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPos = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
    
}
