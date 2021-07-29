using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using blu.FileIO;
using System;

namespace blu
{
    public class IOModule : Module
    {
        public SaveData ActiveSaveData
        { get; set; }

        public const int MaxSaveFiles = 4;

        public SaveSlotData[] SaveSlots
        { get; private set; }

        public string ApplicationPath
        { get; private set; }

        private string m_activeSavedataPath = null;
        private const string m_kSaveGameDir = "SavedGames/";

        public bool IsSaveLoading
        { get; private set; }

        public bool IsSaving
        { get; private set; }

        public bool IsSaveLoaded
        { get { return ActiveSaveData != null; } }

        public bool Initialised
        { get; private set; }

        public Task AwaitInitialised() => Task.Run(() => AwaitInitialisedImpl());

        public Task AwaitSaveLoaded() => Task.Run(() => AwaitSaveLoadedImpl());

        private void AwaitInitialisedImpl()
        {
            while (!Initialised) { }
        }

        private void AwaitSaveLoadedImpl()
        {
            while (!IsSaveLoaded) { }
        }

        public Task<bool> LoadSaveAsync(FileIO.SaveSlotData slotData, bool logToConsole = true) => Task.Run(() => LoadSaveAsyncImpl(slotData, logToConsole));

        public Task<bool> CreateNewSave(int slot, bool loadSave) => Task.Run(() => CreateNewSaveImpl(slot, loadSave));

        public Task<bool> SaveAsync() => Task.Run(() => SaveAsyncImpl());

        public override void Initialize()
        {
            Initialised = false;
            IsSaveLoading = false;

            Debug.Log("[App]: Initializing IO module");
            ApplicationPath = Application.persistentDataPath;

            SaveSlots = new SaveSlotData[MaxSaveFiles];
            LoadSaveSlots();

            Initialised = true;
        }

        public void DiscardSaveData()
        {
            ActiveSaveData = null;
            m_activeSavedataPath = null;
        }

        private List<string> GetAllFilesOfType<T>(string directory = "") where T : IFileFormat, new()
        {
            string dir = ApplicationPath + "/" + directory;

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

        private BaseFileLoader<T> CreateFileLoader<T>(string path) where T : class, IFileFormat
        {
            return new DebugFileLoader<T>(path);
        }

        public bool LoadSaveSlots()
        {
            for (int i = SaveSlots.Length - 1; i >= 0; i--)
            {
                SaveSlots[i] = null;
            }

            FileLoaderStaticUtility.CreateDirectory(ApplicationPath + "/" + m_kSaveGameDir);

            try
            {
                List<string> files = GetAllFilesOfType<SaveData>(m_kSaveGameDir);

                while (files.Count > MaxSaveFiles)
                {
                    files.RemoveAt(files.Count - 1);
                }

                for (int i = 0; i < files.Count; i++)
                {
                    FileIO.SaveSlotData data = new FileIO.SaveSlotData();
                    data.m_filepath = files[i];

                    BaseFileLoader<SaveData> fileloader = CreateFileLoader<SaveData>(data.m_filepath);
                    SaveData savedata = fileloader.ReadData();

                    data.levelId = savedata.levelId;
                    data.playtime = savedata.playtime;

                    if (savedata == null)
                    {
                        Debug.LogWarning($"[App/IOModule.LoadSaveSlots] could not load savedata");
                    }

                    int slot = savedata.saveSlot;
                    int startSlot = slot;

                    while (true)
                    {
                        if (slot >= SaveSlots.Length)
                            slot = 0;

                        if (SaveSlots[slot] == null)
                        {
                            SaveSlots[slot] = data;
                            break;
                        }

                        slot++;

                        if (slot == startSlot)
                        {
                            Debug.LogWarning("[App/IOModule] error assigning savedata to save slot");
                            break;
                        }
                    }

                    if (slot != startSlot)
                    {
                        savedata.saveSlot = slot;
                        fileloader.WriteData(savedata); // set new slot
                    }
                }
            }
            catch
            {
                Debug.LogWarning("[App/IOModule]: Error while loading Save Slots");
                return false;
            }

            return true;
        }

        private bool LoadSaveAsyncImpl(FileIO.SaveSlotData slotData, bool logToConsole = true)
        {
            if (IsSaveLoading)
                return false;

            IsSaveLoading = true;
            while (!Initialised)
            {
            }

            DiscardSaveData();

            m_activeSavedataPath = slotData.m_filepath;
            ActiveSaveData = null;

            BaseFileLoader<SaveData> fileloader = CreateFileLoader<SaveData>(m_activeSavedataPath);
            SaveData savedata = fileloader.ReadData();

            if (savedata == null)
            {
                m_activeSavedataPath = null;
                IsSaveLoading = false;
                return false;
            }
            ActiveSaveData = savedata;

            if (logToConsole)
            { Debug.Log($"Save file loaded: [File = {m_activeSavedataPath}]"); }

            IsSaveLoading = false;
            return true;
        }

        internal bool CreateNewSaveImpl(int slot, bool loadSave)
        {
            System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;

            SaveData savedata = new SaveData();
            savedata.saveSlot = slot;

            string filename = m_kSaveGameDir + cur_time.ToString();
            string filepath = ApplicationPath + "/" + filename + "." + savedata.FileExtension();
            BaseFileLoader<SaveData> fileloader = CreateFileLoader<SaveData>(filepath);

            if (fileloader.FileExists())
            { return false; }

            if (!fileloader.WriteData(savedata))
            { return false; }

            if (loadSave)
            {
                ActiveSaveData = savedata;
                m_activeSavedataPath = filepath;
            }

            return LoadSaveSlots();
        }

        private bool SaveAsyncImpl()
        {
            if (IsSaving)
                return false;

            IsSaving = true;
            bool success = false;

            if (m_activeSavedataPath != null && ActiveSaveData != null)
            {
                BaseFileLoader<SaveData> fileloader = CreateFileLoader<SaveData>(m_activeSavedataPath);
                success = fileloader.WriteData(ActiveSaveData);
            }

            IsSaving = false;
            return success;
        }
    }
}