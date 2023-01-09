using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseEnabler : MonoBehaviour
{
    public GameManager.Phase enablePhase;
    
    void Start()
    {
        GameManager.Singleton.OnPhaseChange += OnPhaseChange;
        OnPhaseChange();
    }

    private void OnPhaseChange()
    {
        if (GameManager.Singleton.currentPhase == enablePhase)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}