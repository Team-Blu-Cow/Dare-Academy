using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace blu.EditorTools
{
    [InitializeOnLoad]
    public class OpenSaveEditor
    {
        public const string MenuName = "Tools/Save Editor";

        [MenuItem(MenuName)]
        private static void ButtonCall()
        {
            SaveEditor window = (SaveEditor)EditorWindow.GetWindow(typeof(SaveEditor), false, "Save Editor");
        }
    }

    public class SaveEditor : EditorWindow
    {
        private string filepath = "";
        private SaveData savedata = null;
        private bool m_flagsFoldoutExpanded = false;
        private Vector2 m_scrollPos = Vector2.zero;
        private Vector2 m_flagsScrollPos = Vector2.zero;

        public void OnValidate()
        {
            if (filepath != null && filepath.Length > 0)
            {
                bool success = OpenFile(filepath);
                if (!success)
                {
                    filepath = "";
                    savedata = null;
                }
            }
        }

        public void OnEnable()
        {
        }

        private bool OpenFile(string path)
        {
            if (path.Length != 0)
            {
                if (System.IO.File.Exists(path))
                {
                    filepath = path;
                    FileIO.BaseFileLoader<SaveData> fileloader = new FileIO.DebugFileLoader<SaveData>(filepath);
                    savedata = fileloader.ReadData();

                    return true;
                }
            }
            return false;
        }

        private bool SaveFile()
        {
            FileIO.BaseFileLoader<SaveData> fileloader = new FileIO.DebugFileLoader<SaveData>(filepath);
            return fileloader.WriteData(savedata);
        }

        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("File Location: ", EditorStyles.boldLabel, GUILayout.Width(100f));
            GUILayout.Label(filepath, EditorStyles.boldLabel);
            if (GUILayout.Button("Find", GUILayout.Width(100f)))
            {
                SaveData savedata = new SaveData();

                string path = EditorUtility.OpenFilePanel("Debug Save File", Application.persistentDataPath, savedata.FileExtension());
                OpenFile(path);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(15);

            if (savedata != null)
            {
                m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos);

                Type type = typeof(SaveData);
                FieldInfo[] fieldInfo = type.GetFields();

                for (int i = 0; i < fieldInfo.Length; i++)
                {
                    object obj = fieldInfo[i].GetValue(savedata);
                    string name = fieldInfo[i].Name;

                    GUILayout.BeginHorizontal();

                    GUILayout.Label(name, GUILayout.Width(100f));

                    switch (name)
                    {
                        case "gameEventFlags":
                            obj = DisplayGameEventFlags((Int32)obj);
                            break;

                        default:
                            // fallback for built in types
                            obj = DefaultFieldHandleing(obj);
                            break;
                    }

                    fieldInfo[i].SetValue(savedata, obj);
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();

                GUILayout.Space(10);
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Save"))
                {
                    SaveFile();
                    OpenFile(filepath);
                }

                if (GUILayout.Button("Reset"))
                {
                    OpenFile(filepath);
                }

                GUILayout.EndHorizontal();
            }
        }

        private void DataNotReadable()
        {
            GUILayout.Label("DATA NOT READABLE IN EDITOR");
        }

        private object DefaultFieldHandleing(object field)
        {
            if (field is bool)
            {
                return GUILayout.Toggle((bool)field, "");
            }

            if (field is int)
            {
                return EditorGUILayout.IntField((int)field);
            }

            if (field is long)
            {
                return EditorGUILayout.LongField((long)field);
            }

            if (field is System.String)
            {
                return EditorGUILayout.TextField((System.String)field);
            }

            if (field is float)
            {
                return EditorGUILayout.FloatField((float)field);
            }

            if (field is double)
            {
                return EditorGUILayout.DoubleField((double)field);
            }

            if (field is Vector2)
            {
                return EditorGUILayout.Vector2Field("", (Vector2)field);
            }

            if (field is Vector2Int)
            {
                return EditorGUILayout.Vector2IntField("", (Vector2Int)field);
            }

            if (field is Vector3)
            {
                return EditorGUILayout.Vector3Field("", (Vector3)field);
            }

            if (field is Vector3Int)
            {
                return EditorGUILayout.Vector3IntField("", (Vector3Int)field);
            }

            if (field is Vector4)
            {
                return EditorGUILayout.Vector4Field("", (Vector4)field);
            }

            DataNotReadable();
            return field;
        }

        private Int32 DisplayGameEventFlags(Int32 gameEventFlags)
        {
            string[] flagNames = System.Enum.GetNames(typeof(GameEventFlags.Flags));
            System.Array flagValues = System.Enum.GetValues(typeof(GameEventFlags.Flags));

            m_flagsFoldoutExpanded = EditorGUILayout.Foldout(m_flagsFoldoutExpanded, "flags");
            if (m_flagsFoldoutExpanded)
            {
                m_flagsScrollPos = EditorGUILayout.BeginScrollView(m_flagsScrollPos, GUILayout.Height(120));
                for (int i = 0; i < flagNames.Length; i++)
                {
                    bool fieldBool = GameEventFlags.IsFlagSet((Int32)flagValues.GetValue(i), gameEventFlags);

                    fieldBool = GUILayout.Toggle(fieldBool, flagNames[i]);
                    GameEventFlags.Flags mask = (GameEventFlags.Flags)flagValues.GetValue(i);
                    gameEventFlags = GameEventFlags.SetFlags((Int32)mask, gameEventFlags, fieldBool);
                }
                EditorGUILayout.EndScrollView();
            }

            return gameEventFlags;
        }
    }
}