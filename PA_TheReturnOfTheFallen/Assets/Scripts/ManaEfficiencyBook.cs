using UnityEngine;

public class ManaEfficiencyBook : MonoBehaviour
{
    //[HideInInspector] public bool PlayerPerto = false;
    private PlayerController playerController;

    //[SerializeField]public float efficiencyRate = 0.2f;
    [SerializeField] public int efficiencyBonusMana = 20;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            // PlayerPerto = false;

            PlayerMana mp = other.GetComponent<PlayerMana>();
            if (mp != null)
            {
                mp.maxMana = mp.maxMana + efficiencyBonusMana;

                Destroy(gameObject);
            }

        }
    }

   /* void Update()
    {
        if (!PlayerPerto) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
              PlayerHP ph = other.GetComponent<PlayerHP>();
              if (ph != null && ph.currentHealth < ph.maxHealth)
              {
                  int effectiveHeal = Mathf.Min(healAmount, ph.maxHealth - ph.currentHealth);
                  ph.Heal(effectiveHeal);

                  Destroy(gameObject);
              }
        }
    }*/
}

