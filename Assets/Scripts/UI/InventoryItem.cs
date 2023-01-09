using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Image icon;
    public TMP_Text amount;
    public TMP_Text cost;
    public Image effectIcon;
    public TMP_Text effectValue;
    public Image sideEffectIcon;
    public TMP_Text sideEffectValue;
    public Toggle toggle;

    public void SetItem(Medicine item, InventoryInfo info)
    {
        name = item.name;
        icon.sprite = item.icon;
        amount.text = info.count.ToString();
        cost.text = Mathf.RoundToInt(info.averageCost).ToString();

        effectIcon.sprite = GameManager.Singleton.GetMedicineElementIcon(item.element);
        effectValue.text = item.elementValue.ToString();

        sideEffectIcon.sprite = GameManager.Singleton.GetMedicineSideEffectIcon(item.sideEffect);
        sideEffectValue.text = $"{item.sideEffectProbability}%";

        if (GameManager.Singleton.selectedMedicine == item)
        {
            toggle.SetIsOnWithoutNotify(true);
        }

        toggle.onValueChanged.AddListener((value) =>
        {
            if (!value && GameManager.Singleton.selectedMedicine == item)
            {
                GameManager.Singleton.selectedMedicine = null;
            }

            if (value)
            {
                GameManager.Singleton.selectedMedicine = item;
            }
        });
    }
}