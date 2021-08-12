using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using TMPro;

public class SaveSlots : MonoBehaviour
{
    private GameObject[] m_slots = new GameObject[IOModule.MaxSaveFiles];
    private IOModule io;

    private void Start()
    {
        io = App.GetModule<IOModule>();

        for (int i = 0; i < IOModule.MaxSaveFiles; i++)
        {
            try
            {
                m_slots[i] = transform.GetChild(i).gameObject;
            }
            catch (System.Exception)
            {
                Debug.LogWarning("[UI/MainMenu.SaveSlot] Not enough children for save slots");
            }
        }

        io.LoadSaveSlots();

        for (int i = 0; i < IOModule.MaxSaveFiles; i++)
        {
            string name;
            if (io.SaveSlots[i] == null)
            {
                name = "StartingCutscene";
                //LevelModule.ResolveSceneNameString(blu.LevelID._default)
            }
            else
            {
                name = LevelModule.ResolveSceneNameString(io.SaveSlots[i].levelId);
            }

            GetComponentInParent<CanvasTool.ButtonWrapper>().buttons[i].sceneName = name;
        }
    }

    public async void OnOpen()
    {
        App.GetModule<LevelModule>().FlushSaveData();
        App.GetModule<LevelModule>().lvlTransitionInfo = null;

        if (io.IsSaveLoaded)
        {
            //#matthew this could cause serious issues with bad data being written
            await io.SaveAsync();
            io.DiscardSaveData();
        }

        io.DiscardSaveData();
        io.LoadSaveSlots();

        for (int i = 0; i < m_slots.Length; i++)
        {
            if (io.SaveSlots[i] == null)
            {
                SaveSlotButton ssb = m_slots[i].GetComponent<SaveSlotButton>();

                ssb.DisableExtras();

                ssb.saveName.text = "New Game";

                //m_slots[i].GetComponentInChildren<TextMeshProUGUI>().text = "New Game";
            }
            else
            {
                SaveSlotButton ssb = m_slots[i].GetComponent<SaveSlotButton>();
                if (ssb != null)
                {
                    ssb.EnableExtras();

                    ssb.SpawnHearts(io.SaveSlots[i].heartCount);
                    ssb.SpawnEnergy(io.SaveSlots[i].enegryCount);

                    ssb.powerupUnlocked[0] = io.SaveSlots[i].powerUpsunlocked[0];
                    ssb.powerupUnlocked[1] = io.SaveSlots[i].powerUpsunlocked[1];
                    ssb.powerupUnlocked[2] = io.SaveSlots[i].powerUpsunlocked[2];

                    ssb.InitPowerups();

                    string playtime = System.TimeSpan.FromSeconds(io.SaveSlots[i].playtime).ToString(@"hh\:mm\:ss");

                    ssb.saveName.text = $"Save {i + 1}";
                    ssb.playTime.text = $"{playtime}";

                    ssb.isInitialised = true;
                }

                //m_slots[i].GetComponentInChildren<TextMeshProUGUI>().text = $"Save {i + 1}          {playtime}";
            }
        }
    }

    public async void OnSave(int slotNumber)
    {
        if (io.SaveSlots[slotNumber] == null)
            await io.CreateNewSave(slotNumber, true);
        else
        {
            await io.LoadSaveAsync(io.SaveSlots[slotNumber]);
        }
    }

    public void OnDelete(int slotNumber)
    {
        if (io.SaveSlots[slotNumber] != null)
        {
            System.IO.File.Delete(io.SaveSlots[slotNumber].m_filepath);
            io.LoadSaveSlots();
            GetComponentInParent<CanvasTool.ButtonWrapper>().buttons[slotNumber].sceneName = "StartingCutscene";
            OnOpen();
        }
    }
}