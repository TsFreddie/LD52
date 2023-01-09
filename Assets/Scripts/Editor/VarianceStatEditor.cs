using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(VarianceStat))]
public class VarianceStatEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var valueRect = new Rect(position.x, position.y, position.width - 55, position.height);
        var plusMinusRect = new Rect(position.x + position.width - 55, position.y, 12, position.height);
        var varianceRect = new Rect(position.x + position.width - 43, position.y, 30, position.height);
        var percentageRect = new Rect(position.x + position.width - 13, position.y, 13, position.height);

        EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("baseValue"), GUIContent.none);
        EditorGUI.LabelField(plusMinusRect, "±");
        EditorGUI.PropertyField(varianceRect, property.FindPropertyRelative("variancePercentage"), GUIContent.none);
        EditorGUI.LabelField(percentageRect, "%");

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}