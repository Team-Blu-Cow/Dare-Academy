using UnityEditor;
using UnityEngine;

namespace JUtil.Grids
{

    [CustomPropertyDrawer(typeof(DataPair<bool>))]
    public class DataPairBoolPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float splitPoint = (position.width / 4) * 3;
            float checkboxPos = splitPoint + (position.width / 8)-5;

            var nameRect = new Rect(position.x, position.y, splitPoint, position.height);
            var dataRect = new Rect(position.x + checkboxPos, position.y, position.width/4, position.height);

            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"),GUIContent.none);
            EditorGUI.PropertyField(dataRect, property.FindPropertyRelative("data"),GUIContent.none);

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(DataPair<int>))]
    public class DataPairIntPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float splitPoint = (position.width / 4) * 3;

            var nameRect = new Rect(position.x, position.y, splitPoint, position.height);
            var dataRect = new Rect(position.x + splitPoint+5, position.y, position.width/4 - 5, position.height);

            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);
            EditorGUI.PropertyField(dataRect, property.FindPropertyRelative("data"), GUIContent.none);

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(DataPair<float>))]
    public class DataPairFloatPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float splitPoint = (position.width / 4) * 3;

            var nameRect = new Rect(position.x, position.y, splitPoint, position.height);
            var dataRect = new Rect(position.x + splitPoint + 5, position.y, position.width / 4 - 5, position.height);

            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);
            EditorGUI.PropertyField(dataRect, property.FindPropertyRelative("data"), GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}