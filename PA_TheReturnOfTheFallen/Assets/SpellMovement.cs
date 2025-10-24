using UnityEngine;

public class SpellMovement : MonoBehaviour
{
    public float speed = 15f;
    public Vector3 direction;

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
}
