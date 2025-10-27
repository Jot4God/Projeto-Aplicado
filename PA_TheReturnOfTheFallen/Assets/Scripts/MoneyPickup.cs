using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    public int amount = 10;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMoney pm = other.GetComponent<PlayerMoney>();
            if (pm != null)
            {
                pm.AddMoney(amount);
                Destroy(gameObject);
            }
        }
    }
}
