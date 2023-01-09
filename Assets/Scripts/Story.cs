using UnityEngine;
using UnityEngine.Serialization;

public enum StoryEnding
{
    [InspectorName("正常存活（顾客可能会再次出现，设置了成功后续故事则再次出现时进行后续故事，未设置则随机）")]
    Survive,

    [InspectorName("后遗症（一定天数后会死亡，再次出现可被救治，设置了失败后续故事则进行失败后续故事，未设置则随机）")]
    AfterEffect,

    [InspectorName("即死（当场死亡，顾客不再出现）")]
    Dead,

    [InspectorName("圆满存活（顾客不会再次出现）")]
    Happy,
}

[System.Serializable]
public class KarmaSequence
{
    [Header("提示文本")]
    public string text;

    [Header("故事结局")]
    public StoryEnding ending;

    [Header("后续故事")]
    public WeightedValue<Story>[] subStories;

    [Header("声望变化")]
    public int prestige;
}

/// <summary>
/// 故事定义
/// </summary>
[CreateAssetMenu(fileName = "New Story", menuName = "LD52数据/故事")]
public class Story : ScriptableObject
{
    [Header("故事 Prefab")]
    public GameObject[] storyPrefab;

    [Header("故事标准收益")]
    public int standardPrice;

    [Header("故事参数最小值（战斗需要的属性值/需要治疗的属性值等）")]
    public int storyValueMin;

    [Header("故事参数最大值")]
    public int storyValueMax;

    [Header("故事需求参数类型")]
    public MedicineElement storyElement;

    [Header("需求的副作用（满足列表任意一种即为成功，若为空则不需要副作用）")]
    public MedicineSideEffect[] successSideEffects;

    // [Header("是否为购买（购买顾客不会要错药）")]
    // public bool isBuy;

    [Header("启用报应（禁用后，下面的设置均不生效，禁用代表该顾客只是想购买什么东西）")]
    public bool enableKarma = false;

    [Header("参数优先（启用后同时满足需求和副作用时，走满足参数的报应，否则走满足副作用的报应）")]
    public bool valueFirst = false;
    
    [Header("报应最低天数")]
    public int karmaMinDay;
    
    [Header("报应最高天数")]
    public int karmaMaxDay;

    [Header("满足参数的报应")]
    public KarmaSequence valueKarma;
    
    [Header("满足副作用的报应（若无需求副作用则不会触发）")]
    public KarmaSequence sideEffectKarma;
    
    [Header("失败的报应")]
    public KarmaSequence failKarma;
}