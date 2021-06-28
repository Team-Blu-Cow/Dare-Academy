using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

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

        public void OnEnable()
        {
        }

        private Dictionary<string, object> ParseJson(string json)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
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
            string json = JsonConvert.SerializeObject(fileData);
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
                foreach (string key in fileData.Keys)
                {
                    // do something with entry.Value or entry.Key
                    GUILayout.BeginHorizontal();

                    GUILayout.Label(key, GUILayout.Width(100f));
                    string oldValue = fileData[key].ToString();

                    // check if object is still json, this will be a subclass
                    // TODO @Matthew - deal with this so it can be modified in editor
                    if (fileData[key].GetType() == new Newtonsoft.Json.Linq.JObject().GetType())
                    {
                        GUILayout.Label("DATA NOT READABLE IN EDITOR");
                        GUILayout.EndHorizontal();
                        continue;
                    }

                    string newValue = GUILayout.TextField(oldValue);

                    GUILayout.EndHorizontal();
                    if (oldValue != newValue)
                    {
                        fileData[key] = (object)newValue;
                        break;
                    }
                }

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
    }
}