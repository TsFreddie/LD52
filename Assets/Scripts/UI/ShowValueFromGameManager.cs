using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowValueFromGameManager : MonoBehaviour
{
    public string property;
    public string format = "{0}";

    void Start()
    {
        GameManager.Singleton.OnStatUpdate += OnStatUpdate;
        OnStatUpdate();
    }

    private void OnStatUpdate()
    {
        var value = GameManager.Singleton.GetType().GetProperty(property).GetValue(GameManager.Singleton);
        GetComponent<TMP_Text>().text = string.Format(format, value);
    }
}