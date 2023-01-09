using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(WeightedValue<>))]
public class WeightedValueEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var weightRect = new Rect(position.x, position.y, 30, position.height);
        var percentageRect = new Rect(position.x + 30, position.y, 12, position.height);
        var valueRect = new Rect(position.x + 52, position.y, position.width - 52, position.height);

        EditorGUI.PropertyField(weightRect, property.FindPropertyRelative("weight"), GUIContent.none);
        EditorGUI.LabelField(percentageRect, "%");
        EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}