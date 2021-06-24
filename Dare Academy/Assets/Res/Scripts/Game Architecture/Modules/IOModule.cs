using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using blu.FileIO;

namespace blu
{
    public class IOModule : Module
    {
        public async override void Initialize()
        {
            Debug.Log("[App]: Initializing IO module");
            m_applicationPath = Application.persistentDataPath;

            Task<bool> taskLoadSaves = Task.Run(() => { return LoadSaveSlots(); });
            Task<DebugSaveConfigFile> taskDebugConfig = Task.Run(() => { return LoadDebugConfig(); });

            m_debugConfig = await taskDebugConfig;
            await taskLoadSaves;

            ProccessDebugConfig();

            m_initialized = true;
        }

        public SaveData savedata => m_activeSavedata;
        public bool isSaveLoaded { get => savedata != null; }
        public List<SaveSlotData> saveSlots { get => m_SaveSlots; }

        private string m_applicationPath = null;
        private Encryptor m_encryptor = new Encryptor(Crypto.FileEncryptionKeys.key, Crypto.FileEncryptionKeys.iv);
        private SaveData m_activeSavedata = null;
        private string m_activeSavedataPath = null;
        private List<SaveSlotData> m_SaveSlots = new List<SaveSlotData>();
        private int m_runtimeIdCounter = 0;
        private const string m_kSaveGameDir = "SavedGames/";

        private DebugSaveConfigFile m_debugConfig = null;
        private bool m_usingDebugFile = false;
        private bool m_allowDebugSaving = false;
        private bool m_allowFileLoading = true;

        // things are happening in async that shouldnt be and i dont want to debug this
        private bool m_initialized = false;

        public Task<bool> LoadSaveAsync(FileIO.SaveSlotData slotData, bool logToConsole = true) => Task.Run(() => LoadSaveAsyncImpl(slotData, logToConsole));

        public Task<bool> CreateNewSave(string displayName) => Task.Run(() => CreateNewSaveImpl(displayName));

        public Task<bool> SaveAsync() => Task.Run(() => SaveAsyncImpl());

        private List<string> GetAllFilesOfType<T>(string directory = "") where T : BaseFileFormat, new()
        {
            string dir = m_applicationPath + "/" + directory;

            string[] files = FileLoaderStaticUtility.GetFilesInDirectory(dir);
            List<string> ret = new List<string>();

            T format = new T();
            string extension = format.FileExtension();

            for (int i = 0; i < files.Length; i++)
            {
                // if extension is correct add it to list
                string[] fileSplit = files[i].Split('.');
                if (fileSplit[fileSplit.Length - 1] == extension)
                {
                    ret.Add(files[i]);
                }
            }

            return ret;
        }

        private bool CreateFileLoader<T>(out BaseFileLoader<T> fileloader, string path) where T : BaseFileFormat
        {
            if (UnityEngine.Application.isEditor)
            { fileloader = new DebugFileLoader<T>(path); }
            else
            { fileloader = new EncryptedFileLoader<T>(path, m_encryptor); }

            if (fileloader != null)
            { return true; }
            else
            { return false; }
        }

        private bool LoadSaveSlots()
        {
            saveSlots.Clear();

            FileLoaderStaticUtility.CreateDirectory(m_applicationPath + "/" + m_kSaveGameDir);

            try
            {
                List<string> files = GetAllFilesOfType<FileIO.SaveData>(m_kSaveGameDir);

                for (int i = 0; i < files.Count; i++)
                {
                    FileIO.SaveSlotData data = new FileIO.SaveSlotData();
                    data.m_filepath = files[i];
                    data.m_runtimeID = m_runtimeIdCounter++;

                    CreateFileLoader<SaveData>(out BaseFileLoader<SaveData> fileloader, data.m_filepath);
                    FileIO.SaveData savedata = fileloader.ReadData();

                    if (savedata == null)
                    { continue; }

                    data.m_displayName = savedata.displayName;

                    m_SaveSlots.Add(data);
                }
            }
            catch (System.Exception ex) //TODO @Matthew: my guy please rename these, IDK what this does and also stop
                                        // leaving them, unhandled
            {
                Debug.LogWarning("[App/SettingsModule]: Error while loading Save Slots"); // not in the settings module my guy
                return false;
            }

            return true;
        }

        private bool LoadSaveAsyncImpl(FileIO.SaveSlotData slotData, bool logToConsole = true)
        {
            while (m_initialized == false)
            {
            }

            if (m_usingDebugFile)
            {
                m_activeSavedataPath = m_debugConfig.debugFileLocation;
            }
            else
            {
                m_activeSavedataPath = slotData.m_filepath;
            }
            m_activeSavedata = null;

            CreateFileLoader<FileIO.SaveData>(out BaseFileLoader<FileIO.SaveData> fileloader, m_activeSavedataPath);

            SaveData savedata = fileloader.ReadData();

            if (savedata == null)
            {
                m_activeSavedataPath = null;
                return false;
            }
            m_activeSavedata = savedata;

            if (logToConsole)
            { Debug.Log($"Save file loaded: [DisplayName = {savedata.displayName}] [File = {m_activeSavedataPath}]"); }  // TODO @Matthew: Use [App/IOModule]:

            return true;
        }

        private bool CreateNewSaveImpl(string displayName)
        {
            // check for duplicate
            for (int i = 0; i < saveSlots.Count; i++)
            {
                if (saveSlots[i].m_displayName == displayName)
                {
                    return false;
                }
            }

            FileIO.SaveSlotData data = new FileIO.SaveSlotData();
            data.m_displayName = displayName;
            data.m_runtimeID = m_runtimeIdCounter++;

            System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;

            SaveData savedata = new SaveData();
            savedata.displayName = data.m_displayName;

            string filename = m_kSaveGameDir + cur_time.ToString();
            string filepath = m_applicationPath + "/" + filename + "." + savedata.FileExtension();
            CreateFileLoader<SaveData>(out BaseFileLoader<SaveData> fileloader, filepath);

            if (fileloader.FileExists())
            { return false; }

            if (!fileloader.WriteData(savedata))
            { return false; }

            return LoadSaveSlots();
        }

        private bool SaveAsyncImpl()
        {
            if (m_usingDebugFile && !m_allowDebugSaving)
            { return false; }

            if (m_activeSavedataPath != null && m_activeSavedata != null)
            {
                CreateFileLoader<FileIO.SaveData>(out BaseFileLoader<FileIO.SaveData> fileloader, m_activeSavedataPath);
                return fileloader.WriteData(m_activeSavedata);
            }
            return false;
        }

        private DebugSaveConfigFile LoadDebugConfig() //TODO @Matthew: sorry for nit picking but can you please use
        {                                             // consistent braces? 209 is a oneliner and 212-215 just changes
            if (!Application.isEditor)                // for no dicernable reason
            { return null; }

            BaseFileLoader<DebugSaveConfigFile> fileloader = new DebugFileLoader<DebugSaveConfigFile>(m_applicationPath + FileIO.DebugSaveConfigFile.path);
            if (fileloader.FileExists())
            {
                return fileloader.ReadData();
            }
            return null;
        }

        private void ProccessDebugConfig()
        {
            if (!Application.isEditor)
            { return; }

            if (m_debugConfig == null)
            { return; }

            m_usingDebugFile = m_debugConfig.enable;
            m_allowDebugSaving = m_debugConfig.allowSaving;
            if (m_usingDebugFile)
            {
                Debug.Log("Using Debug Save File");

                BaseFileLoader<SaveData> fileloader = new DebugFileLoader<SaveData>(m_debugConfig.debugFileLocation);
                if (fileloader.FileExists())
                {
                    if (m_debugConfig.loadOnStart)
                    {
                        m_activeSavedata = fileloader.ReadData();
                        m_activeSavedataPath = m_debugConfig.debugFileLocation;
                        Debug.Log($"Save file loaded: [DisplayName = {savedata.displayName}] [File = {m_activeSavedataPath}]"); // TODO @Matthew: see 153:
                    }
                }
                else
                {
                    m_usingDebugFile = false;
                    Debug.LogWarning("Could not find Debug Save File"); // TODO @Matthew: see 153:
                }
            }
        }
    }
}