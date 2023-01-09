using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyListItem : MonoBehaviour
{
    public Image icon;
    public Image newIcon;
    public Image trend;
    public TMP_Text priceText;
    public TMP_Text prevPriceText;
    public Image effectIcon;
    public TMP_Text effectValue;
    public Image sideEffectIcon;
    public TMP_Text sideEffectValue;
    public Button buyButton;

    /// <summary>
    /// prevPrice: -1 = new item
    /// </summary>
    public void SetItem(Medicine item, int price, int prevPrice)
    {
        name = item.name;
        icon.sprite = item.icon;
        priceText.text = price.ToString();
        prevPriceText.text = prevPrice.ToString();
        newIcon.gameObject.SetActive(false);

        if (price == prevPrice)
        {
            trend.gameObject.SetActive(false);
            prevPriceText.gameObject.SetActive(false);
        }
        else if (prevPrice < 0)
        {
            trend.gameObject.SetActive(false);
            prevPriceText.gameObject.SetActive(false);
            newIcon.gameObject.SetActive(true);
        }
        else if (price > prevPrice)
        {
            trend.color = new Color32(128, 0, 0, 255);
            trend.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            trend.color = new Color32(0, 128, 0, 255);
            trend.rectTransform.rotation = Quaternion.Euler(0, 0, 180);
        }

        effectIcon.sprite = GameManager.Singleton.GetMedicineElementIcon(item.element);
        effectValue.text = item.elementValue.ToString();

        sideEffectIcon.sprite = GameManager.Singleton.GetMedicineSideEffectIcon(item.sideEffect);
        sideEffectValue.text = $"{item.sideEffectProbability}%";

        buyButton.onClick.AddListener(() => { GameManager.Singleton.BuyMedicine(item, price); });
    }
}