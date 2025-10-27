using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class PlayerMoney : MonoBehaviour
{
    public int currentMoney = 0;
    public TextMeshProUGUI moneyText; 

    void Start()
    {
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
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
        moneyText.text = currentMoney.ToString();
    }
}
