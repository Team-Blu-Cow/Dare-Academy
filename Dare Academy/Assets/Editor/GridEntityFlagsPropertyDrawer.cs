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
            height += 5 + EditorGUIUtility.singleLineHeight * (BitFlagsBase.NumberOfFlags<GridEntityFlags.Flags>() + 2);

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

        JUtil.PropertyDrawerUtility.IndentLevel++;

        SerializedProperty flagDataProp = property.FindPropertyRelative("m_flagData");

        util.NewLine();
        Rect rect = util.GetSingleLineRect();

        string hexValue = flagDataProp.intValue.ToString("X");
        hexValue = EditorGUI.TextField(rect, hexValue);
        flagDataProp.intValue = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);

        string[] flagNames = BitFlagsBase.FlagNames<GridEntityFlags.Flags>();

        System.Array flagValues = System.Enum.GetValues(typeof(GridEntityFlags.Flags));

        for (int i = 0; i < BitFlagsBase.NumberOfFlags<GridEntityFlags.Flags>(); i++)
        {
            util.NewLine();

            bool fieldBool = GridEntityFlags.IsFlagSet((int)flagValues.GetValue(i), flagDataProp.intValue);

            rect = util.GetSingleLineRect();

            fieldBool = EditorGUI.Toggle(rect, flagNames[i], fieldBool);

            GridEntityFlags.Flags mask = (GridEntityFlags.Flags)flagValues.GetValue(i);

            flagDataProp.intValue = (int)GridEntityFlags.SetFlags((int)mask, (int)flagDataProp.intValue, fieldBool);
        }

        JUtil.PropertyDrawerUtility.IndentLevel--;
        EditorGUI.EndProperty();
    }
}//*/