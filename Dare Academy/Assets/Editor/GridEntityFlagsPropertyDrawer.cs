using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(GridEntityFlags))]
public class GridEntityFlagsPropertyDrawer : PropertyDrawer
{
    private JUtil.PropertyDrawerUtility util;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;

        if (property.isExpanded)
            height += EditorGUIUtility.singleLineHeight * (GridEntityFlags.NumberOfFlags()+1) + (EditorGUIUtility.singleLineHeight/2);

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        util = new JUtil.PropertyDrawerUtility(position);

        EditorGUI.BeginProperty(position, label, property);

        Rect dropdown = util.GetSingleLineRect();
        property.isExpanded = EditorGUI.Foldout(dropdown, property.isExpanded, property.displayName, true);
        if (!property.isExpanded)
        {
            EditorGUI.EndProperty();
            return;
        }

        SerializedProperty flagDataProp = property.FindPropertyRelative("m_flagData");

        util.NewLine();
        Rect rect = util.GetSingleLineRect();
        EditorGUI.IntField(rect, flagDataProp.intValue);

        string[] flagNames = flagDataProp.enumNames;
        System.Array flagValues = System.Enum.GetValues(typeof(GridEntityFlags.Flags));

        for (int i = 0; i < GridEntityFlags.NumberOfFlags(); i++)
        {
            util.NewLine();

            bool fieldBool = GridEntityFlags.IsFlagSet((GridEntityFlags.Flags)flagValues.GetValue(i), (uint)flagDataProp.intValue);

            rect = util.GetSingleLineRect();

            fieldBool = EditorGUI.Toggle(rect, flagNames[i], fieldBool);

            GridEntityFlags.Flags mask = (GridEntityFlags.Flags)flagValues.GetValue(i);

            flagDataProp.intValue = (int)GridEntityFlags.Toggle(mask, (uint)flagDataProp.intValue, fieldBool);
        }

        EditorGUI.EndProperty();
    }
}//*/