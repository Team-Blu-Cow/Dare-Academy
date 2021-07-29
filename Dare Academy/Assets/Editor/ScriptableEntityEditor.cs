using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(ScriptableEntity))]
public class ScriptableEntityEditor : Editor
{
    private enum _boolEnum
    {
        False = 0,
        True = 1,
    }

    private enum _cameraOptionsEnum
    {
        gameobject,
        vec3,
    }

    private enum _anyallEnum
    {
        any,
        all,
    }

    private SerializedProperty prefabProperty;

    private bool flagFoldout = false;
    private bool actionFoldout = false;

    private void OnValidate()
    {
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        prefabProperty = serializedObject.FindProperty("m_prefab");
        EditorGUILayout.PropertyField(prefabProperty);

        SerializedProperty actionQueueProperty = serializedObject.FindProperty("m_actionQueue");
        SerializedProperty flagsProperty = serializedObject.FindProperty("m_flagValue");

        flagsProperty.intValue = DisplayFlags(flagsProperty.intValue);

        if (actionQueueProperty.objectReferenceValue == null)
        {
            actionQueueProperty.objectReferenceValue = ScriptableObject.CreateInstance<ScriptedActionQueue>();
        }

        actionFoldout = EditorGUILayout.Foldout(actionFoldout, "Actions");

        if (actionFoldout)
        {
            ScriptedActionQueue actionQueue = actionQueueProperty.objectReferenceValue as ScriptedActionQueue;
            if (DisplayActionQueue(ref actionQueue))
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            actionQueueProperty.objectReferenceValue = actionQueue;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private bool DisplayActionQueue(ref ScriptedActionQueue queue)
    {
        bool dirty = false;

        EditorGUILayout.BeginHorizontal();

        int len = EditorGUILayout.IntField(queue.m_actionList.Count);

        if (GUILayout.Button("+", GUILayout.Width(24f)))
        {
            dirty = true;
            len++;
        }

        if (GUILayout.Button("-", GUILayout.Width(24f)))
        {
            dirty = true;
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
                queue.m_actionList[i].int32Data = 0;
                queue.m_actionList[i].textData = "";
                queue.m_actionList[i].moveData = new ScriptedActionQueue.MoveData();
                queue.m_actionList[i].gameObject = null;
                queue.m_actionList[i].boolData = false;
                queue.m_actionList[i].vec3data = Vector3.zero;
            }

            switch (queue.m_actionList[i].type)
            {
                case ScriptedActionQueue.ActionType.None:
                    break;

                case ScriptedActionQueue.ActionType.WaitTurns:
                    queue.m_actionList[i].int32Data = IntField((int?)(queue.m_actionList[i].int32Data));
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

                case ScriptedActionQueue.ActionType.SetFlagEntityValue:
                    EntityFlagField(ref queue.m_actionList[i].int32Data, ref queue.m_actionList[i].boolData);
                    break;

                case ScriptedActionQueue.ActionType.SetEventFlagValue:
                    EventFlagField(ref queue.m_actionList[i].int32Data, ref queue.m_actionList[i].boolData);
                    break;

                case ScriptedActionQueue.ActionType.SetCameraPosition:
                    SetCameraPositionField(ref queue.m_actionList[i].boolData, ref queue.m_actionList[i].vec3data, ref queue.m_actionList[i].gameObject);
                    break;

                case ScriptedActionQueue.ActionType.ExecuteSteps:
                    queue.m_actionList[i].int32Data = IntField(queue.m_actionList[i].int32Data);
                    queue.m_actionList[i].doubleData = DoubleField(queue.m_actionList[i].doubleData);
                    break;

                case ScriptedActionQueue.ActionType.KillIfEventFlagSet:
                    KillIfEventFlagSet(ref queue.m_actionList[i].int32Data, ref queue.m_actionList[i].boolData);
                    break;

                case ScriptedActionQueue.ActionType.KillIfEventFlagNotSet:
                    KillIfEventFlagSet(ref queue.m_actionList[i].int32Data, ref queue.m_actionList[i].boolData);
                    break;

                default:
                    break;
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("▲", GUILayout.Width(24f)))
            {
                if (i > 0)
                {
                    dirty = true;
                    ScriptedActionQueue.ActionWrapper temp = queue.m_actionList[i];
                    queue.m_actionList[i] = queue.m_actionList[i - 1];
                    queue.m_actionList[i - 1] = temp;
                }
            }

            if (GUILayout.Button("▼", GUILayout.Width(24f)))
            {
                if (i < queue.m_actionList.Count - 1)
                {
                    dirty = true;
                    ScriptedActionQueue.ActionWrapper temp = queue.m_actionList[i];
                    queue.m_actionList[i] = queue.m_actionList[i + 1];
                    queue.m_actionList[i + 1] = temp;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        return dirty;
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

    private double DoubleField(double? d)
    {
        if (d == null)
        { d = new double(); }

        d = EditorGUILayout.DoubleField((double)d);

        if (d < 0)
            d = 0;

        return (double)d;
    }

    private GameObject ObjectField(GameObject obj)
    {
        return EditorGUILayout.ObjectField(obj, typeof(GameObject), true) as GameObject;
    }

    private ScriptedActionQueue.MoveData MoveDataField(ScriptedActionQueue.MoveData data)
    {
        if (data == null)
            data = new ScriptedActionQueue.MoveData();

        data.direction = (ScriptedActionQueue.MoveDirections)EditorGUILayout.EnumPopup(data.direction, GUILayout.Width(100));

        data.distance = IntField(data.distance);

        return (ScriptedActionQueue.MoveData)data;
    }

    private void EntityFlagField(ref int i, ref bool b)
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

    private void EventFlagField(ref int i, ref bool b)
    {
        i = (int)(GameEventFlags.Flags)EditorGUILayout.EnumFlagsField((GameEventFlags.Flags)(i), GUILayout.Width(160f));

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

    private void KillIfEventFlagSet(ref int i, ref bool b)
    {
        i = (int)(GameEventFlags.Flags)EditorGUILayout.EnumFlagsField((GameEventFlags.Flags)(i), GUILayout.Width(160f));

        _anyallEnum e;
        if (b)
            e = _anyallEnum.all;
        else
            e = _anyallEnum.any;

        e = (_anyallEnum)EditorGUILayout.EnumPopup(e, GUILayout.Width(100f));

        if (e == _anyallEnum.all)
            b = true;
        else
            b = false;
    }

    private void SetCameraPositionField(ref bool b, ref Vector3 vec3, ref GameObject obj)
    {
        _cameraOptionsEnum e;
        if (b)
            e = _cameraOptionsEnum.gameobject;
        else
            e = _cameraOptionsEnum.vec3;

        e = (_cameraOptionsEnum)EditorGUILayout.EnumPopup(e, GUILayout.Width(100f));

        if (e == _cameraOptionsEnum.gameobject)
            b = true;
        else
            b = false;

        if (b)
        {
            obj = ObjectField(obj);
            vec3 = Vector3.zero;
        }
        else
        {
            obj = null;
            vec3 = EditorGUILayout.Vector3Field("", vec3);
        }
    }

    private int DisplayFlags(int value)
    {
        flagFoldout = EditorGUILayout.Foldout(flagFoldout, "Flags");
        if (flagFoldout)
        {
            string[] flagNames = BitFlagsBase.FlagNames<GridEntityFlags.Flags>();

            System.Array flagValues = System.Enum.GetValues(typeof(GridEntityFlags.Flags));

            for (int i = 0; i < BitFlagsBase.NumberOfFlags<GridEntityFlags.Flags>(); i++)
            {
                bool fieldBool = GridEntityFlags.IsFlagSet((int)flagValues.GetValue(i), value);

                fieldBool = EditorGUILayout.Toggle(flagNames[i], fieldBool);

                int mask = (int)flagValues.GetValue(i);

                value = GridEntityFlags.SetFlags(mask, value, fieldBool);
            }
        }
        return value;
    }
}