using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyListManager : MonoBehaviour
{
    [SerializeField] private GameObject _itemPrefab;

    private Transform _content;

    private List<Medicine> _medicines;

    void Start()
    {
        _content = GetComponent<ScrollRect>().content;
        _medicines = new List<Medicine>();
        GameManager.Singleton.OnBuyListUpdate += UpdateBuyList;
        UpdateBuyList();
    }

    private void UpdateBuyList()
    {
        foreach (Transform child in _content)
        {
            Destroy(child.gameObject);
        }

        GameManager.Singleton.GetBuyList(_medicines);
        _medicines.Sort((a, b) => a.name.CompareTo(b.name));

        foreach (var item in _medicines)
        {
            var itemObject = Instantiate(_itemPrefab, _content);
            var itemComponent = itemObject.GetComponent<BuyListItem>();
            var price = GameManager.Singleton.GetPrice(item);
            var prevPrice = GameManager.Singleton.GetPrevPrice(item);
            itemComponent.SetItem(item, price, prevPrice);
        }
    }
}