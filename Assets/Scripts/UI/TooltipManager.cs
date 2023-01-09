using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public RectTransform tooltip;
    
    /// <summary>
    /// Singleton instance of the GameManager
    /// </summary>
    public static GameManager Singleton { get; private set; }
}
