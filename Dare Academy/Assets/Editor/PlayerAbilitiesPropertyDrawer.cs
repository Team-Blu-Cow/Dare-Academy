using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(PlayerAbilities))]
public class PlayerAbilitiesPropertyDrawer : PropertyDrawer
{
    private JUtil.PropertyDrawerUtility util;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        util = new JUtil.PropertyDrawerUtility(position);

        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty activeAbility = property.FindPropertyRelative("m_activeAbility");

        int intValue = activeAbility.enumValueIndex;
        PlayerAbilities.AbilityEnum enumValue = (PlayerAbilities.AbilityEnum)(intValue);

        Rect rect = util.GetSingleLineRect();
        EditorGUI.LabelField(rect, $"Active Ability: {enumValue.ToString()}");

        EditorGUI.EndProperty();
    }
}