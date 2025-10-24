using UnityEngine;
using System.Collections;

public class PlayerDash : MonoBehaviour
{
    public float dashDistance = 5f;       
    public float dashDuration = 0.15f;    
    public float dashCooldown = 0.5f;     

    private bool canDash = true;
    private Rigidbody rd;
    private PlayerController controller;

    void Start()
    {
        rd = GetComponent<Rigidbody>();
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        canDash = false;

        controller.podeMover = false;

        Vector3 inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 dashDir = inputDir.normalized;
        if (dashDir == Vector3.zero)
            dashDir = transform.forward;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + dashDir * dashDistance;
        float elapsed = 0f;

        // Opcional: ignora colisões
        Collider col = GetComponent<Collider>();
        col.enabled = false;

        while (elapsed < dashDuration)
        {
            rd.MovePosition(Vector3.Lerp(startPos, targetPos, elapsed / dashDuration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        rd.MovePosition(targetPos);
        col.enabled = true;
        controller.podeMover = true;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
