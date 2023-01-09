using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject _itemPrefab;

    private Transform _content;

    private List<Medicine> _medicines;

    void Start()
    {
        _content = GetComponent<ScrollRect>().content;
        _medicines = new List<Medicine>();
        GameManager.Singleton.OnInventoryUpdate += UpdateInventory;
        UpdateInventory();
    }

    private void UpdateInventory()
    {
        foreach (Transform child in _content)
        {
            Destroy(child.gameObject);
        }

        GameManager.Singleton.GetInventoryList(_medicines);
        _medicines.Sort((a, b) => a.name.CompareTo(b.name));

        foreach (var item in _medicines)
        {
            var info = GameManager.Singleton.GetInventoryInfo(item);
            var itemObject = Instantiate(_itemPrefab, _content);
            var itemComponent = itemObject.GetComponent<InventoryItem>();
            itemComponent.SetItem(item, info);
        }
    }
}