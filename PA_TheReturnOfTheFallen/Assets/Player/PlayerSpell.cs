using UnityEngine;

public class PlayerSpell : MonoBehaviour
{
    public GameObject spellPrefab;
    public int manaCost = 10;
    public float spellCooldown = 0.3f;

    private bool canCast = true;
    private PlayerMana playerMana;
    private PlayerAttack playerAttack;
    private Camera mainCam;

    void Start()
    {
        playerMana = GetComponent<PlayerMana>();
        playerAttack = GetComponent<PlayerAttack>();
        mainCam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && canCast)
        {
            CastSpell();
        }
    }

    void CastSpell()
    {
        if (playerMana != null && playerMana.UseMana(manaCost))
        {
            canCast = false;

            Vector3 spawnPos = playerAttack.attackPoint.position;

            // Direção para o clique do mouse
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 direction = (hitPoint - spawnPos).normalized;
                direction.y = 0;

                GameObject spell = Instantiate(spellPrefab, spawnPos, Quaternion.identity);

                SpellMovement sm = spell.GetComponent<SpellMovement>();
                if (sm != null)
                    sm.SetDirection(direction);

                Destroy(spell, 3f); // destrói após 3s
            }

            Invoke(nameof(ResetCast), spellCooldown);
        }
        else
        {
            Debug.Log("❌ Sem mana suficiente!");
        }
    }

    void ResetCast()
    {
        canCast = true;
    }
}
