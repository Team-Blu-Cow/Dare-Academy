using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using blu.FileIO;
using System;

namespace blu
{
    // All data that should be written to disk should be stored within SaveData
    // Everything in here must be tagged with System.Serializable
    public class SaveData : IFileFormat, ICloneable
    {
        public string FileExtension()
        {
            if (UnityEngine.Application.isEditor)
            { return "uwu-debug"; }
            else
            { return "uwu"; }
        }

        // when adding new members ensure you add them to Clone()
        public object Clone()
        {
            SaveData data = new SaveData();

            data.displayName = this.displayName;
            data.levelId = this.levelId;
            data.respawnRoomID = this.respawnRoomID;

            return data;
        }

        public string displayName = null;
        public LevelID levelId = LevelID._default;
        public int respawnRoomID = -1;
    }

    public class IOModule : Module
    {
        // the current save file that has been loaded from disk
        public SaveData savedata { get { return m_activeSavedata; } set { m_activeSavedata = value; } }

        // check if a valid save file has been loaded
        public bool isSaveLoaded { get => savedata != null; }

        // if save slots are currently loaded
        public bool isSaveSlotsLoaded { get => m_saveSlotsLoaded; }

        // a list of every save slot available to the player
        // the SaveSlotData structure can be handed to LoadSaveAsync to load the associated file into memory
        public List<SaveSlotData> saveSlots { get => m_SaveSlots; }

        private string m_applicationPath = null;
        private Encryptor m_encryptor = new Encryptor(Crypto.FileEncryptionKeys.key, Crypto.FileEncryptionKeys.iv);
        private SaveData m_activeSavedata = null;
        private string m_activeSavedataPath = null;
        private List<SaveSlotData> m_SaveSlots = new List<SaveSlotData>();
        private int m_runtimeIdCounter = 0;
        private const string m_kSaveGameDir = "SavedGames/";
        private bool m_saveSlotsLoaded = false;
        private DebugSaveConfigFile m_debugConfig = null;
        private bool m_usingDebugFile = false;
        private bool m_allowDebugSaving = false;
        // TODO @matthew add this to the debug options
        // private bool m_allowFileLoading = true;

        // things are happening in async that shouldn't be and i dont want to debug this
        private bool m_initialized = false;

        public bool Initialised => m_initialized;

        public Task<bool> awaitInitialised => Task.Run(() => awaitInitialisedImpl());

        private bool awaitInitialisedImpl()
        {
            while (!Initialised)
            { }
            return true;
        }

        // load a save file into memory
        public Task<bool> LoadSaveAsync(FileIO.SaveSlotData slotData, bool logToConsole = true) => Task.Run(() => LoadSaveAsyncImpl(slotData, logToConsole));

        // load file in a coroutine
        // if you are using this method to load files, ensure you check the isSaveLoaded flag when accessing data
        public IEnumerator LoadSaveEnumerator(FileIO.SaveSlotData slotData, bool logToConsole = true)
        { yield return LoadSaveAsync(slotData, logToConsole); }

        // creates a new save file
        public Task<bool> CreateNewSave(string displayName) => Task.Run(() => CreateNewSaveImpl(displayName));

        // write the current contents of saveData to disk
        public Task<bool> SaveAsync() => Task.Run(() => SaveAsyncImpl());

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

        public bool DiscardSaveData()
        {
            m_activeSavedata = null;
            m_activeSavedataPath = null;

            return true;
        }

        private List<string> GetAllFilesOfType<T>(string directory = "") where T : IFileFormat, new()
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

        private bool CreateFileLoader<T>(out BaseFileLoader<T> fileloader, string path) where T : class, IFileFormat
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
            m_saveSlotsLoaded = false;
            saveSlots.Clear();

            FileLoaderStaticUtility.CreateDirectory(m_applicationPath + "/" + m_kSaveGameDir);

            try
            {
                List<string> files = GetAllFilesOfType<SaveData>(m_kSaveGameDir);

                for (int i = 0; i < files.Count; i++)
                {
                    FileIO.SaveSlotData data = new FileIO.SaveSlotData();
                    data.m_filepath = files[i];
                    data.m_runtimeID = m_runtimeIdCounter++;

                    CreateFileLoader<SaveData>(out BaseFileLoader<SaveData> fileloader, data.m_filepath);
                    SaveData savedata = fileloader.ReadData();

                    if (savedata == null)
                    { continue; }

                    data.m_displayName = savedata.displayName;

                    m_SaveSlots.Add(data);
                }
            }
            catch
            {
                Debug.LogWarning("[App/IOModule]: Error while loading Save Slots");
                return false;
            }

            m_saveSlotsLoaded = true;
            return true;
        }

        private bool LoadSaveAsyncImpl(FileIO.SaveSlotData slotData, bool logToConsole = true)
        {
            while (m_initialized == false)
            {
            }

            DiscardSaveData();

            if (m_usingDebugFile)
            {
                m_activeSavedataPath = m_debugConfig.debugFileLocation;
            }
            else
            {
                m_activeSavedataPath = slotData.m_filepath;
            }
            m_activeSavedata = null;

            CreateFileLoader<SaveData>(out BaseFileLoader<SaveData> fileloader, m_activeSavedataPath);

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
            // for (int i = 0; i < saveSlots.Count; i++)
            // {
            //     if (saveSlots[i].m_displayName == displayName)
            //     {
            //         return false;
            //     }
            // }

            System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;

            SaveData savedata = new SaveData();
            savedata.displayName = displayName;

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
                CreateFileLoader<SaveData>(out BaseFileLoader<SaveData> fileloader, m_activeSavedataPath);
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