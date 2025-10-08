using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;   
    public float speed = 3f;   

    void Start()

    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()

    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}
