using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI")]
    public Slider healthBar;
    public Text healthText;

    [Header("Armadura")]
    public PlayerArmor playerArmor;

    [Header("Audio - Heal")]
    public AudioSource healAudioSource;   // AudioSource no Player (ou noutro objeto)
    public AudioClip healSfx;             // Som quando recebe vida
    [Range(0f, 1f)] public float healVolume = 1f;

    private PlayerRespawn respawn;

    void Awake()
    {
        currentHealth = maxHealth;

        if (healthBar == null)
        {
            GameObject hb = GameObject.FindGameObjectWithTag("HealthBar");
            if (hb != null) healthBar = hb.GetComponent<Slider>();
        }

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (playerArmor == null)
            playerArmor = GetComponent<PlayerArmor>();

        respawn = GetComponent<PlayerRespawn>();

        // Se não arrastares no Inspector, tenta buscar um AudioSource no Player
        if (healAudioSource == null)
            healAudioSource = GetComponent<AudioSource>();

        UpdateHealthText();
    }

    void Update()
    {
        if (healthBar != null)
        {
            if (healthBar.maxValue != maxHealth) healthBar.maxValue = maxHealth;
            if (healthBar.value != currentHealth) healthBar.value = currentHealth;
        }

        UpdateHealthText();
    }

    void UpdateHealthText()
    {
        if (healthText != null)
            healthText.text = currentHealth + " / " + maxHealth;
    }

    public void UpdateUIInstant()
    {
        if (healthBar != null)
            healthBar.value = currentHealth;
        UpdateHealthText();
    }

    public void TakeDamage(int damage)
    {
        int armor = (playerArmor != null ? playerArmor.currentArmor : 0);

        int finalDamage = Mathf.Max(damage - armor, 0);

        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"Dano recebido: {damage} | Armor: {armor} | Final: {finalDamage}");

        if (healthBar != null)
            healthBar.value = currentHealth;

        UpdateHealthText();

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        // Só toca som se realmente curou alguma coisa
        int before = currentHealth;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        if (healthBar != null) healthBar.value = currentHealth;
        UpdateHealthText();

        if (currentHealth > before) // houve cura efetiva
            PlayHealSfx();
    }

    private void PlayHealSfx()
    {
        if (healAudioSource == null || healSfx == null) return;

        // Usa o AudioSource (não é PlayClipAtPoint)
        healAudioSource.PlayOneShot(healSfx, healVolume);
    }

    void Die()
    {
        GetComponent<PlayerMana>().tempManaCount = 0;
        GetComponent<PlayerMoney>().currentMoney = 0;

        Debug.Log("Player morreu!");

        if (respawn != null)
        {
            respawn.HandleDeath();
            return;
        }

        ForceGameOver();
    }

    public void ForceGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
}
