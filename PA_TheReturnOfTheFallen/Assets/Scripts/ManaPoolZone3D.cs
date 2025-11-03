using UnityEngine;

public class ManaPoolZone3D : MonoBehaviour
{
    public int maxUses = 5;
    public int manaAmount = 10;
    public float cooldown = 1f;

    private int currentUses;
    private float lastHealTime = 0f;

    void Start()
    {
        currentUses = maxUses;
    }

    void OnTriggerStay(Collider other)
    {
        if (currentUses <= 0) return;
        if (Time.time - lastHealTime < cooldown) return;

        PlayerMana playerMana = other.GetComponent<PlayerMana>();
        if (playerMana != null)
        {
            playerMana.RestoreMana(manaAmount);
            currentUses--;
            lastHealTime = Time.time;

            if (currentUses <= 0)
                DisablePool();
        }
    }

    void DisablePool()
    {
        Debug.Log("Mana Pool esgotada!");
        // opcional: muda a cor para cinza
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
            rend.material.color = Color.gray;

        // desativa o trigger
        GetComponent<Collider>().enabled = false;
    }
}
