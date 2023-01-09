using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomerStats : MonoBehaviour
{
    public RectTransform healthBar;

    public TMP_Text healthText;

    public TMP_Text attackText;

    public TMP_Text shieldText;

    public TMP_Text charmText;
    
    void Start()
    {
        GameManager.Singleton.OnCustomerUpdate += UpdateCustomerStats;
        UpdateCustomerStats();
    }

    private void UpdateCustomerStats()
    {
        var customer = GameManager.Singleton.currentCustomer;
        if (customer == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        gameObject.SetActive(true);
        healthBar.anchorMax = new Vector2((float)customer.customerHealth / customer.customerMaxHealth, 1);
        healthText.text = $"{customer.customerHealth}/{customer.customerMaxHealth}";
        attackText.text = customer.customerAttack.ToString();
        shieldText.text = customer.customerShield.ToString();
        charmText.text = customer.customerCharm.ToString();
    }
}
