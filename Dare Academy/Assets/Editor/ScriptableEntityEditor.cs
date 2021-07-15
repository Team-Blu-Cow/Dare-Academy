using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScriptableEntity))]
public class ScriptableEntityEditor : Editor
{
    private SerializedProperty prefabProperty;

    private bool foldout = false;

    private void OnValidate()
    {
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        prefabProperty = serializedObject.FindProperty("m_prefab");

        SerializedProperty actionQueueProperty = serializedObject.FindProperty("m_actionQueue");

        if (actionQueueProperty.objectReferenceValue == null)
        {
            actionQueueProperty.objectReferenceValue = ScriptableObject.CreateInstance<ScriptedActionQueue>();
        }

        EditorGUILayout.PropertyField(prefabProperty);

        foldout = EditorGUILayout.Foldout(foldout, "Actions");

        if (foldout)
        {
            ScriptedActionQueue actionQueue = actionQueueProperty.objectReferenceValue as ScriptedActionQueue;
            DisplayActionQueue(ref actionQueue);
            actionQueueProperty.objectReferenceValue = actionQueue;
        }

        EditorUtility.SetDirty(serializedObject.targetObject);

        serializedObject.ApplyModifiedProperties();
    }

    private void DisplayActionQueue(ref ScriptedActionQueue queue)
    {
        if (queue.m_actionList == null)
        {
            queue.m_actionList = new List<ScriptedActionQueue.ActionWrapper>();
        }

        EditorGUILayout.BeginHorizontal();

        int len = EditorGUILayout.IntField(queue.m_actionList.Count);

        if (GUILayout.Button("+", GUILayout.Width(20f)))
        {
            len++;
        }

        if (GUILayout.Button("-", GUILayout.Width(20f)))
        {
            len--;
        }

        EditorGUILayout.EndHorizontal();

        if (len < 0)
            len = 0;

        while (queue.m_actionList.Count > len)
        {
            if (queue.m_actionList.Count == 0)
            { break; }

            queue.m_actionList.RemoveAt(queue.m_actionList.Count - 1);
        }

        while (queue.m_actionList.Count < len)
        {
            queue.m_actionList.Add(new ScriptedActionQueue.ActionWrapper());
        }

        // enum-dropdown, input-field
        for (int i = 0; i < queue.m_actionList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            ScriptedActionQueue.ActionType currentType = queue.m_actionList[i].type;

            ScriptedActionQueue.ActionType newType = (ScriptedActionQueue.ActionType)EditorGUILayout.EnumPopup(currentType, GUILayout.Width(160f));

            if (currentType != newType)
            {
                queue.m_actionList[i].type = newType;
                queue.m_actionList[i].intData = 0;
                queue.m_actionList[i].textData = "";
                queue.m_actionList[i].moveData = new ScriptedActionQueue.MoveData();
                queue.m_actionList[i].gameObject = null;
            }

            switch (queue.m_actionList[i].type)
            {
                case ScriptedActionQueue.ActionType.None:
                    break;

                case ScriptedActionQueue.ActionType.WaitTurns:
                    queue.m_actionList[i].intData = IntField((int?)(queue.m_actionList[i].intData));
                    break;

                case ScriptedActionQueue.ActionType.WaitPlayerIsDistance:
                    queue.m_actionList[i].intData = IntField((int?)(queue.m_actionList[i].intData));
                    break;

                case ScriptedActionQueue.ActionType.Move:
                    queue.m_actionList[i].moveData = MoveDataField(queue.m_actionList[i].moveData);
                    break;

                case ScriptedActionQueue.ActionType.Dialogue:

                    queue.m_actionList[i].gameObject = EditorGUILayout.ObjectField(queue.m_actionList[i].gameObject, typeof(GameObject), true) as GameObject;
                    break;

                default:
                    break;
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private string StringField(string s)
    {
        if (s == null)
        { s = ""; }

        s = EditorGUILayout.TextField(s);

        return s;
    }

    private int IntField(int? i)
    {
        if (i == null)
        { i = new int(); }

        i = EditorGUILayout.IntField((int)i);

        if (i < 0)
            i = 0;

        return (int)i;
    }

    private ScriptedActionQueue.MoveData MoveDataField(ScriptedActionQueue.MoveData data)
    {
        if (data == null)
            data = new ScriptedActionQueue.MoveData();

        data.direction = (ScriptedActionQueue.MoveDirections)EditorGUILayout.EnumPopup(data.direction, GUILayout.Width(100));

        data.distance = IntField(data.distance);

        return (ScriptedActionQueue.MoveData)data;
    }
}