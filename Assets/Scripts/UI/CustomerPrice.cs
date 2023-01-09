using TMPro;
using UnityEngine;

public class CustomerPrice : MonoBehaviour
{
    public TMP_Text price;

    void Start()
    {
        GameManager.Singleton.OnCustomerUpdate += UpdateCustomerPrice;
        UpdateCustomerPrice();
    }

    private void UpdateCustomerPrice()
    {
        var customer = GameManager.Singleton.currentCustomer;
        if (customer == null)
        {
            price.text = "-";
            return;
        }

        price.text = customer.priceValue.ToString();
    }
}