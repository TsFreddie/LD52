using UnityEngine;

/// <summary>
/// 进度规则
/// </summary>
[CreateAssetMenu(fileName = "New Ruleset", menuName = "LD52数据/进度规则")]
public class ProgressionRuleset : ScriptableObject
{
    [Header("可购入草药")]
    public Medicine[] availableMedicines;
    
    [Header("可出现的顾客")]
    public Customer[] availableCustomers;
}
