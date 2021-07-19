using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScriptableEntity))]
public class ScriptableEntityEditor : Editor
{
    private enum _boolEnum
    {
        False = 0,
        True = 1,
    }

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

        serializedObject.ApplyModifiedProperties();
    }

    private void DisplayActionQueue(ref ScriptedActionQueue queue)
    {
        EditorGUILayout.BeginHorizontal();

        int len = EditorGUILayout.IntField(queue.m_actionList.Count);

        if (GUILayout.Button("+", GUILayout.Width(24f)))
        {
            len++;
        }

        if (GUILayout.Button("-", GUILayout.Width(24f)))
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
                queue.m_actionList[i].boolData = false;
            }

            switch (queue.m_actionList[i].type)
            {
                case ScriptedActionQueue.ActionType.None:
                    break;

                case ScriptedActionQueue.ActionType.WaitTurns:
                    queue.m_actionList[i].intData = IntField((int?)(queue.m_actionList[i].intData));
                    break;

                case ScriptedActionQueue.ActionType.Move:
                    queue.m_actionList[i].moveData = MoveDataField(queue.m_actionList[i].moveData);
                    break;

                case ScriptedActionQueue.ActionType.Dialogue:
                    queue.m_actionList[i].gameObject = EditorGUILayout.ObjectField(queue.m_actionList[i].gameObject, typeof(GameObject), true) as GameObject;
                    break;

                case ScriptedActionQueue.ActionType.WaitPlayerEnterTrigger:
                    queue.m_actionList[i].gameObject = EditorGUILayout.ObjectField(queue.m_actionList[i].gameObject, typeof(GameObject), true) as GameObject;
                    break;

                case ScriptedActionQueue.ActionType.WaitPlayerExitTrigger:
                    queue.m_actionList[i].gameObject = EditorGUILayout.ObjectField(queue.m_actionList[i].gameObject, typeof(GameObject), true) as GameObject;
                    break;

                case ScriptedActionQueue.ActionType.SetFlagValue:
                    FlagField(ref queue.m_actionList[i].intData, ref queue.m_actionList[i].boolData);
                    break;

                default:
                    break;
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("▲", GUILayout.Width(24f)))
            {
                if (i > 0)
                {
                    ScriptedActionQueue.ActionWrapper temp = queue.m_actionList[i];
                    queue.m_actionList[i] = queue.m_actionList[i - 1];
                    queue.m_actionList[i - 1] = temp;
                }
            }

            if (GUILayout.Button("▼", GUILayout.Width(24f)))
            {
                if (i < queue.m_actionList.Count - 1)
                {
                    ScriptedActionQueue.ActionWrapper temp = queue.m_actionList[i];
                    queue.m_actionList[i] = queue.m_actionList[i + 1];
                    queue.m_actionList[i + 1] = temp;
                }
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

    private void FlagField(ref int i, ref bool b)
    {
        i = (int)(GridEntityFlags.Flags)EditorGUILayout.EnumFlagsField((GridEntityFlags.Flags)(i), GUILayout.Width(160f));

        _boolEnum e;
        if (b)
            e = _boolEnum.True;
        else
            e = _boolEnum.False;

        e = (_boolEnum)EditorGUILayout.EnumPopup(e, GUILayout.Width(100f));

        if (e == _boolEnum.True)
            b = true;
        else
            b = false;
    }
}