/// <summary>
/// 权值类型
/// </summary>
[System.Serializable]
public struct WeightedValue<T>
{
    public float weight;
    public T value;
}