using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using System;

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
        private Dictionary<string, object> fileData = null;
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
                    fileData = null;
                }
            }
        }

        public void OnEnable()
        {
        }

        private Dictionary<string, object> ParseJson(string json)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }

        private string SerializeJson(Dictionary<string, object> structure)
        {
            return JsonConvert.SerializeObject(structure);
        }

        private bool OpenFile(string path)
        {
            if (path.Length != 0)
            {
                if (System.IO.File.Exists(path))
                {
                    // ensure latest version of file format is used
                    FileIO.BaseFileLoader<SaveData> fileloader = new FileIO.DebugFileLoader<SaveData>(path);
                    SaveData savedata = fileloader.ReadData();
                    fileloader.WriteData(savedata);

                    filepath = path;
                    string json = System.IO.File.ReadAllText(filepath);
                    fileData = ParseJson(json);
                    return true;
                }
            }
            return false;
        }

        private bool SaveFile()
        {
            string json = SerializeJson(fileData);
            System.IO.File.WriteAllText(filepath, json);
            return true;
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

            if (fileData != null)
            {
                m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos);

                string[] keys = new string[fileData.Keys.Count];
                fileData.Keys.CopyTo(keys, 0);
                for (int k = 0; k < keys.Length; k++)
                {
                    // do something with entry.Value or entry.Key
                    GUILayout.BeginHorizontal();

                    GUILayout.Label(keys[k], GUILayout.Width(100f));

                    // check if object is still json, this will be a subclass
                    // TODO @Matthew - deal with this so it can be modified in editor
                    if (fileData[keys[k]].GetType() == new Newtonsoft.Json.Linq.JObject().GetType())
                    {
                        DataNotReadable();
                        GUILayout.EndHorizontal();
                        continue;
                    }

                    switch (keys[k])
                    {
                        case "gameEventFlags":
                            fileData[keys[k]] = DisplayGameEventFlags((Int64)fileData[keys[k]]);
                            break;

                        default:
                            // fallback for built in types
                            fileData[keys[k]] = DefaultFieldHandleing(fileData[keys[k]]);
                            break;
                    }

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
            System.Type type = fileData.GetType();

            if (field is bool)
            {
                return GUILayout.Toggle((bool)field, "");
            }

            if (field is int)
            {
                field = EditorGUILayout.IntField((int)field);
            }

            return GUILayout.TextField(field.ToString());
        }

        private Int64 DisplayGameEventFlags(Int64 gameEventFlags)
        {
            string[] flagNames = System.Enum.GetNames(typeof(GameEventFlags.Flags));
            System.Array flagValues = System.Enum.GetValues(typeof(GameEventFlags.Flags));

            m_flagsFoldoutExpanded = EditorGUILayout.Foldout(m_flagsFoldoutExpanded, "flags");
            if (m_flagsFoldoutExpanded)
            {
                m_flagsScrollPos = EditorGUILayout.BeginScrollView(m_flagsScrollPos, GUILayout.Height(120));
                for (int i = 0; i < flagNames.Length; i++)
                {
                    bool fieldBool = GameEventFlags.IsFlagSet((Int64)flagValues.GetValue(i), gameEventFlags);

                    fieldBool = GUILayout.Toggle(fieldBool, flagNames[i]);
                    GameEventFlags.Flags mask = (GameEventFlags.Flags)flagValues.GetValue(i);
                    gameEventFlags = GameEventFlags.SetFlags((Int64)mask, gameEventFlags, fieldBool);
                }
                EditorGUILayout.EndScrollView();
            }

            return gameEventFlags;
        }
    }
}