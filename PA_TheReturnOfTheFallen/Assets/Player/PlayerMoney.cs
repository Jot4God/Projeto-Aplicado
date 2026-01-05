using UnityEngine;
using TMPro;

public class PlayerMoney : MonoBehaviour
{
    [Header("Money")]
    public int currentMoney = 0;
    public TextMeshProUGUI moneyText;

    [Header("Coin SFX")]
    [Tooltip("AudioSource no Player (2D/3D conforme quiseres).")]
    public AudioSource sfxSource;

    [Tooltip("Som que toca quando apanhas uma coin.")]
    public AudioClip coinPickupClip;

    [Range(0f, 1f)] public float coinPickupVolume = 1f;
    [Range(0.8f, 1.2f)] public float pitchMin = 0.95f;
    [Range(0.8f, 1.2f)] public float pitchMax = 1.05f;

    void Start()
    {
        // Se não arrastares no Inspector, tenta usar o AudioSource do próprio Player
        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();

        // Toca som só quando realmente adiciona dinheiro (apanhar coin)
        PlayCoinSFX();
    }

    public void SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyUI();
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = currentMoney.ToString();
    }

    private void PlayCoinSFX()
    {
        if (sfxSource == null || coinPickupClip == null) return;

        // Pequena variação para não soar repetitivo
        sfxSource.pitch = Random.Range(pitchMin, pitchMax);
        sfxSource.PlayOneShot(coinPickupClip, coinPickupVolume);
    }
}
