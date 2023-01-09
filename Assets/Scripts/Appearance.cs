using UnityEngine;

/// <summary>
/// 顾客外表
/// </summary>
[CreateAssetMenu(fileName = "New Appearance", menuName = "LD52数据/顾客外表")]
public class Appearance : ScriptableObject
{
    public GameObject[] bodies;
    public GameObject[] heads;
}