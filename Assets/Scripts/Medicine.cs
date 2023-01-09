using UnityEngine;

public enum MedicineType
{
    [InspectorName("无效果")]
    Null,

    [InspectorName("增益")]
    Enhancement,

    [InspectorName("减益")]
    Reduction,
}

public enum MedicineElement
{
    [InspectorName("无能力")]
    Null,

    [InspectorName("生命能力")]
    Health,
    
    [InspectorName("攻击能力")]
    Attack,

    [InspectorName("盾系能力")]
    Shield,

    [InspectorName("运气能力")]
    Charm,

    // [InspectorName("火系能力")]
    // Fire,
    //
    // [InspectorName("水系能力")]
    // Water,
    //
    // [InspectorName("土系能力")]
    // Earth,
}

public enum MedicineSideEffect
{
    [InspectorName("无副作用")]
    Null,

    [InspectorName("头晕")]
    Dizzy,

    [InspectorName("呕吐")]
    Vomit,

    // [InspectorName("中毒")]
    // Poisoning,
    
    // [InspectorName("失血")]
    // Bleeding,
    //
    [InspectorName("猝死")]
    Death,
}

/// <summary>
/// 药材定义
/// </summary>
[CreateAssetMenu(fileName = "Medicine", menuName = "LD52数据/药材")]
public class Medicine : ScriptableObject
{
    [Header("药材类型")]
    public MedicineType type;

    [Header("药材效果")]
    public MedicineElement element;

    [Header("药材效果参数")]
    public int elementValue;
    
    [Header("药材副作用")]
    public MedicineSideEffect sideEffect;
    
    [Header("药材副作用出现概率(%)")]
    public float sideEffectProbability;

    [Header("药材图标")]
    public Sprite icon;

    [Header("药材价格")]
    public VarianceStat cost;
}