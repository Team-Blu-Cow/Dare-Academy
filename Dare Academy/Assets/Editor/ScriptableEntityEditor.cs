using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using ActionQueue = ScriptableEntity.ActionQueue;

[CustomEditor(typeof(ScriptableEntity))]
public class ScriptableEntityEditor : Editor
{
    private SerializedProperty prefabProperty;
    private SerializedProperty scriptedActionListProperty;

    private bool foldout = false;

    //     private void OnEnable()
    //     {
    //         prefabProperty = serializedObject.FindProperty("m_prefab");
    //         scriptedActionListProperty = serializedObject.FindProperty("m_scriptedActionList");
    //     }

    private void OnValidate()
    {
        prefabProperty = serializedObject.FindProperty("m_prefab");
        scriptedActionListProperty = serializedObject.FindProperty("m_scriptedActionList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty actionQueueProperty = serializedObject.FindProperty("m_actionQueue");

        if (actionQueueProperty.objectReferenceValue == null)
        {
            actionQueueProperty.objectReferenceValue = ScriptableObject.CreateInstance<ActionQueue>();
        }

        ActionQueue actionQueue = actionQueueProperty.objectReferenceValue as ActionQueue;
        DisplayActionQueue(ref actionQueue);
        actionQueueProperty.objectReferenceValue = actionQueue;

        EditorGUILayout.PropertyField(prefabProperty);

        foldout = EditorGUILayout.Foldout(foldout, "Actions");

        serializedObject.ApplyModifiedProperties();
    }

    private void DisplayActionQueue(ref ActionQueue queue)
    {
        if (queue.m_actionList == null)
        {
            queue.m_actionList = new List<System.Tuple<ActionQueue.ActionType, object>>();
        }

        int len = EditorGUILayout.IntField(queue.m_actionList.Count);

        while (queue.m_actionList.Count > len)
        {
            if (queue.m_actionList.Count == 0)
            { break; }

            queue.m_actionList.RemoveAt(queue.m_actionList.Count - 1);
        }

        while (queue.m_actionList.Count < len)
        {
            System.Tuple <ActionQueue.ActionType, object> newEntry = new System.Tuple<ActionQueue.ActionType, object>(ActionQueue.ActionType.None, null);

            queue.m_actionList.Add(newEntry);
        }

        // enum-dropdown, input-field
        for (int i = 0; i < queue.m_actionList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            // ActionQueue.ActionType currentType = queue.m_actionList[i].Item1;
            //
            // currentType = (ActionQueue.ActionType)EditorGUILayout.EnumPopup(currentType);
            // queue.m_actionList[i].Item1 = currentType;
            EditorGUILayout.EndHorizontal();
        }
    }
}