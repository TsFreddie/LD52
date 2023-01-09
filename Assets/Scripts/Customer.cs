using UnityEngine;

/// <summary>
/// 顾客类型
/// </summary>
[CreateAssetMenu(fileName = "New Customer", menuName = "LD52数据/顾客")]
public class Customer : ScriptableObject
{
    [Header("顾客故事概率")]
    public WeightedValue<Story>[] stories;

    [Header("顾客属性了解能力(%) 0-100")]
    [Header("顾客猜中自己需求的概率")]
    public float elementKnowledge;

    [Header("顾客溢价能力下限(%) 100代表原价")]
    public float minPremium;

    [Header("顾客溢价能力上限(%) 100代表原价")]
    public float maxPremium;
}