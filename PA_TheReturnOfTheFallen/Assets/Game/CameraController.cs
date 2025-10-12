using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    private Vector3 offset;
    public float smoothSpeed = 5f;

    void OnEnable()
    {
        // Quando a cena recarrega, volta a procurar o Player
        FindPlayer();
    }

    void Start()
    {
        FindPlayer();
    }

    void LateUpdate()
    {
        if (player == null)
        {
            // Se o player for destruído e recriado, tenta encontrá-lo de novo
            FindPlayer();
            return;
        }

        Vector3 targetPos = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }

    void FindPlayer()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            offset = transform.position - player.position;
        }
        else
        {
            Debug.LogWarning("Player não encontrado pela câmera!");
        }
    }
}
