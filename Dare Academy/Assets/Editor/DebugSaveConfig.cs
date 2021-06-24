using UnityEngine;
using UnityEditor;

namespace blu.EditorTools
{
    [InitializeOnLoad]
    public class OpenDebugSaveConfig
    {
        public const string MenuName = "Tools/Debug Save Config";

        [MenuItem(MenuName)]
        private static void ButtonCall()
        {
            DebugSaveConfig window = (DebugSaveConfig)EditorWindow.GetWindow(typeof(DebugSaveConfig), false, "Debug Save File Configuration");
        }
    }

    public class DebugSaveConfig : EditorWindow
    {
        private FileIO.DebugSaveConfigFile config = new FileIO.DebugSaveConfigFile();

        public void OnEnable()
        {
            LoadFromDisk();
        }

        public void OnGUI()
        {
            GUILayout.Label("Debug Save File", EditorStyles.boldLabel);
            GUILayout.Space(10);
            GUILayout.Label(Application.persistentDataPath + FileIO.DebugSaveConfigFile.path);

            GUILayout.BeginHorizontal();
            GUILayout.Label("File Location: ", GUILayout.Width(100f));
            config.debugFileLocation = GUILayout.TextField(config.debugFileLocation);
            if (GUILayout.Button("Find", GUILayout.Width(100f)))
            {
                FileIO.SaveData savedata = new FileIO.SaveData();

                string path = EditorUtility.OpenFilePanel("Debug Save File", Application.persistentDataPath, savedata.FileExtension());
                if (path.Length != 0)
                {
                    config.debugFileLocation = path;
                }
            }
            GUILayout.EndHorizontal();

            config.enable = GUILayout.Toggle(config.enable, "Use Debug File");
            config.allowSaving = GUILayout.Toggle(config.allowSaving, "Allow Saving");
            config.loadOnStart = GUILayout.Toggle(config.loadOnStart, "Load on Start");

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save"))
            {
                SaveToDisk();
            }

            if (GUILayout.Button("Reset"))
            {
                LoadFromDisk();
            }

            GUILayout.EndHorizontal();
        }

        public void LoadFromDisk()
        {
            string path = Application.persistentDataPath + FileIO.DebugSaveConfigFile.path;
            blu.FileIO.BaseFileLoader<FileIO.DebugSaveConfigFile> fileloader = new blu.FileIO.DebugFileLoader<FileIO.DebugSaveConfigFile>(path);
            config = null;
            if (fileloader.FileExists())
            {
                config = fileloader.ReadData();
            }

            if (config == null)
            {
                config = new FileIO.DebugSaveConfigFile();
            }
        }

        public void SaveToDisk()
        {
            FileIO.FileLoaderStaticUtility.CreateDirectory(Application.persistentDataPath + "/Editor/");
            string path = Application.persistentDataPath + FileIO.DebugSaveConfigFile.path;

            blu.FileIO.BaseFileLoader<FileIO.DebugSaveConfigFile> fileloader = new blu.FileIO.DebugFileLoader<FileIO.DebugSaveConfigFile>(path);
            fileloader.WriteData(config);
        }
    }
}