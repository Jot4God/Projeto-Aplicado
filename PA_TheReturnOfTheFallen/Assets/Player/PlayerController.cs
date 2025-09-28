using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float groundDist = 1f;
    public LayerMask terrainLayer;
    public Rigidbody rd;
    public SpriteRenderer sr;

    void Start()
    {
        rd = GetComponent<Rigidbody>();
        rd.constraints = RigidbodyConstraints.FreezeRotation; 
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal"); // A/D
        float z = Input.GetAxis("Vertical");   // W/S

        Vector3 moveDir = new Vector3(x, 0, z).normalized;

        rd.linearVelocity = new Vector3(moveDir.x * speed, rd.linearVelocity.y, moveDir.z * speed);

        if (x < 0)
            sr.flipX = true;
        else if (x > 0)
            sr.flipX = false;
    }

    void FixedUpdate()
    {
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, Mathf.Infinity, terrainLayer))
        {
            float targetY = hit.point.y + groundDist;
            Vector3 pos = rd.position;
            pos.y = Mathf.Lerp(pos.y, targetY, 0.2f); 
            rd.MovePosition(pos); 
        }
    }
}
