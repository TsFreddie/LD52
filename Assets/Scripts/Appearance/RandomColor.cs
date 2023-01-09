using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RandomColor : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("测试颜色")]
    public int testColor;
#endif
    public Color[] colors;

    void Start()
    {
        if (!Application.isPlaying) return;
        if (colors == null || colors.Length <= 0) return;

        var randomIndex = Random.Range(0, colors.Length);
        GetComponent<Image>().color = colors[randomIndex];
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Application.isPlaying) return;
        if (testColor >= 0 && testColor < colors.Length)
        {
            GetComponent<Image>().color = colors[testColor];
        }
    }
#endif
}