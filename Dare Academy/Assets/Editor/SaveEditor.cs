using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using blu.FileIO;

namespace blu.EditorTools
{
    [InitializeOnLoad]
    internal class OpenSaveEditor
    {
        public const string MenuName = "Tools/Save Editor";

        [MenuItem(MenuName, priority = (int)blu.EditorTools.EditorMenuItemPriority.SaveEditor)]
        private static void ButtonCall()
        {
            SaveEditor window = (SaveEditor)EditorWindow.GetWindow(typeof(SaveEditor), false, "Save Editor");
        }
    }

    public class SaveEditor : EditorWindow
    {
        private string m_filepath = "";
        private SaveData m_savedata = null;
        private Vector2 m_scrollPos = Vector2.zero;
        private bool m_doLiveUpdate = false;

        private bool m_flagsFoldoutExpanded = false;
        private Vector2 m_flagsScrollPos = Vector2.zero;

        public void OnEnable()
        {
            m_savedata = null;
            OpenLastFile();

            m_doLiveUpdate = PlayerPrefs.GetInt("Editor_SaveEditor_LiveUpdate", 0) == 1;
        }

        public void OnGUI()
        {
            FileBox();

            if (Application.isPlaying)
                GUI.enabled = false;

            if (m_savedata != null)
                Buttons();

            GUI.enabled = true;

            ToggleLiveUpdate();

            if (Application.isPlaying)
                GUI.enabled = false;

            // buttons could have nulled savedata
            if (m_savedata != null)
            {
                GUILayout.Space(20);

                m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos);

                FieldInfo[] fieldInfo = typeof(SaveData).GetFields();
                for (int i = 0; i < fieldInfo.Length; i++)
                {
                    ShowField(fieldInfo[i]);
                }

                GUILayout.EndScrollView();
            }
        }

        public void Update()
        {
            if (Application.isPlaying && m_doLiveUpdate)
                OpenFile(m_filepath);
        }

        // OnGUI

        private void FileBox()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("File Location: ", EditorStyles.boldLabel, GUILayout.Width(100f));
            GUILayout.Label(m_filepath, EditorStyles.boldLabel);
            if (GUILayout.Button("Find", GUILayout.Width(100f)))
            {
                SaveData savedata = new SaveData();

                string path = EditorUtility.OpenFilePanel("Debug Save File", Application.persistentDataPath, savedata.FileExtension());
                OpenFile(path);
            }
            GUILayout.EndHorizontal();
        }

        private void ToggleLiveUpdate()
        {
            EditorGUILayout.BeginHorizontal();
            GUIContent content = new GUIContent("Live Update", "File will regularly be reloaded at runtime");
            GUILayout.Label(content, GUILayout.Width(100f));
            bool toggle = blu.EditorTools.EditorUtil.BoolField(m_doLiveUpdate);
            EditorGUILayout.EndHorizontal();

            if (toggle != m_doLiveUpdate)
            {
                m_doLiveUpdate = toggle;
                PlayerPrefs.SetInt("Editor_SaveEditor_LiveUpdate", m_doLiveUpdate ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        // returns if a custom behaviour is implemented for this field
        private bool CustomFieldBehaviour(string fieldName, ref object obj)
        {
            switch (fieldName)
            {
                case "gameEventFlags":
                    obj = DisplayGameEventFlags((int)obj);
                    return true;

                case "levelId":
                    obj = DisplayLevelId((blu.LevelID)obj);
                    return true;

                default:
                    // cant find custom behaviour, revert to normal handling
                    return false;
            }

            ;
        }

        private void ShowField(FieldInfo fieldInfo)
        {
            object? obj = fieldInfo.GetValue(m_savedata);
            string name = fieldInfo.Name;

            GUILayout.BeginHorizontal();

            GUILayout.Label(name, GUILayout.Width(100f));

            if (!CustomFieldBehaviour(name, ref obj))
            {
                // no custom behaviour, revert to normal processing
                obj = EditorUtil.AutoFieldHandleing(obj);
            }

            fieldInfo.SetValue(m_savedata, obj);
            GUILayout.EndHorizontal();
        }

        private void Buttons()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(100));
            if (GUILayout.Button("Save", GUILayout.Width(80)))
            {
                SaveFile();
                OpenFile(m_filepath);
            }

            if (GUILayout.Button("Reload", GUILayout.Width(80)))
            {
                OpenFile(m_filepath);
            }

            if (GUILayout.Button("Close", GUILayout.Width(80)))
            {
                CloseFile();
            }

            GUILayout.EndHorizontal();
        }

        // run is the application is playing
        private void AppPlaying()
        {
            GUI.enabled = false;
            OpenFile(m_filepath);
        }

        // FILE IO
        private bool OpenFile(string path)
        {
            if (path != null && path.Length != 0)
            {
                if (System.IO.File.Exists(path))
                {
                    BaseFileLoader<SaveData > fileloader = new JsonFileLoader<SaveData>(path);
                    m_savedata = fileloader.ReadData();
                    if (m_savedata != null)
                    {
                        m_filepath = path;
                        SetLastFile();
                        return true;
                    }
                }
            }
            return false;
        }

        private void CloseFile()
        {
            m_savedata = null;
            m_filepath = null;
            m_scrollPos = Vector2.zero;
            ClearLastFile();
        }

        private bool SaveFile()
        {
            BaseFileLoader<SaveData> fileloader = new JsonFileLoader<SaveData>(m_filepath);
            return fileloader.WriteData(m_savedata);
        }

        // Player Pref

        private void OpenLastFile()
        {
            string lastFilePath = PlayerPrefs.GetString("Editor_SaveEditor_LastFilePath");
            if (lastFilePath != "")
            {
                OpenFile(lastFilePath);
                if (m_filepath == null)
                    ClearLastFile();
            }
        }

        private void SetLastFile()
        {
            if (m_filepath != null)
            {
                PlayerPrefs.SetString("Editor_SaveEditor_LastFilePath", m_filepath);
                PlayerPrefs.Save();
            }
            else
            {
                ClearLastFile();
            }
        }

        private void ClearLastFile()
        {
            PlayerPrefs.DeleteKey("Editor_SaveEditor_LastFilePath");
            PlayerPrefs.Save();
        }

        // CUSTOM FILED HANDLING

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

        private blu.LevelID DisplayLevelId(blu.LevelID levelid)
        {
            return (blu.LevelID)EditorGUILayout.EnumPopup(levelid);
        }
    }
}