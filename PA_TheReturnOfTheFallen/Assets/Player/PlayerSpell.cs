using UnityEngine;

public class PlayerSpell : MonoBehaviour
{
    [Header("Spell")]
    public GameObject spellPrefab;
    public int manaCost = 10;
    public float spellCooldown = 0.3f;

    [Header("SFX (One Shot ao lançar spell)")]
    public AudioClip castSfx;
    [Range(0f, 1f)] public float castSfxVolume = 1f;

    [Tooltip("Opcional. Se vazio, tenta usar AudioSource no player; se não houver, cria um temporário.")]
    public AudioSource sfxSource;

    private bool canCast = true;
    private PlayerMana playerMana;
    private PlayerAttack playerAttack;
    private Camera mainCam;

    void Start()
    {
        playerMana = GetComponent<PlayerMana>();
        playerAttack = GetComponent<PlayerAttack>();
        mainCam = Camera.main;

        // Se não atribuíram no Inspector, tenta apanhar do próprio player
        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && canCast)
        {
            CastSpell();
        }
        if (Input.GetMouseButtonDown(2) && canCast)
        {
            CastSpell1();
        }
    }

    void CastSpell()
    {
        if (playerMana != null && playerMana.UseMana(manaCost))
        {
            canCast = false;

            Vector3 spawnPos = playerAttack.attackPoint.position;

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

                Destroy(spell, 3f);

                // ✅ Som ao lançar
                PlayCastSfx();
            }

            Invoke(nameof(ResetCast), spellCooldown);
        }
        else
        {
            Debug.Log("❌ Sem mana suficiente!");
        }
    }

    void CastSpell1() // Dark Gust
    {
        if (playerMana != null && playerMana.UseMana(manaCost))
        {
            canCast = false;

            Vector3 spawnPos = playerAttack.attackPoint.position;

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

                Destroy(spell, 3f);

                // ✅ Som ao lançar
                PlayCastSfx();
            }

            Invoke(nameof(ResetCast), spellCooldown);
        }
        else
        {
            Debug.Log("❌ Sem mana suficiente!");
        }
    }

    private void PlayCastSfx()
    {
        if (castSfx == null) return;

        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(castSfx, castSfxVolume);
            return;
        }

        // Fallback: cria um AudioSource temporário 2D
        GameObject go = new GameObject("SpellCastSFX_Temp");
        go.transform.position = transform.position;

        AudioSource temp = go.AddComponent<AudioSource>();
        temp.spatialBlend = 0f; // 2D
        temp.playOnAwake = false;

        temp.PlayOneShot(castSfx, castSfxVolume);
        Destroy(go, castSfx.length + 0.1f);
    }

    void ResetCast()
    {
        canCast = true;
    }
}
