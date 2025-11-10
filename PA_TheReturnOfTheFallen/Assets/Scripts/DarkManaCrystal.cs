using UnityEngine;

public class DarkManaCrystal : MonoBehaviour
{
    /*[HideInInspector] public bool PlayerPerto = false;*/
    private PlayerController playerController;
    [SerializeField] public int DarkManaBoost = 50;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            // PlayerPerto = false;

            PlayerMana mp = other.GetComponent<PlayerMana>();
            if (mp != null)
            {
                mp.maxMana = mp.maxMana + DarkManaBoost;
                mp.tempManaCount = mp.tempManaCount + 1;

                Destroy(gameObject);
            }

        }
    }

   /* void Update()
     {
         if (!PlayerPerto) return;

         if (Input.GetKeyDown(KeyCode.E))
         {
               PlayerMana mp = other.GetComponent<PlayerMana>();
               if (mp != null)
               {
                   mp.maxMana = mp.maxMana + DarkManaBoost;
                   mp.tempManaCount = mp.tempManaCount + 1;

                   Destroy(gameObject);
               }
         }
     }*/
}

