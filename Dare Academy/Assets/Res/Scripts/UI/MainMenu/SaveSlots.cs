using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using TMPro;

public class SaveSlots : MonoBehaviour
{
    private GameObject[] m_slots = new GameObject[IOModule.MaxSaveFiles];
    private IOModule io;

    // Start is called before the first frame update
    private async void Start()
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

        await io.awaitInitialised;
        for (int i = 0; i < IOModule.MaxSaveFiles; i++)
        {
            string name;
            if (io.saveSlots[i] == null)
            {
                name = LevelModule.ResolveSceneNameString(blu.LevelID._default);
            }
            else
            {
                name = LevelModule.ResolveSceneNameString(io.saveSlots[i].levelId);
            }

            GetComponentInParent<CanvasTool.ButtonWrapper>().buttons[i].sceneName = name;
        }
    }

    public async void OnOpen()
    {
        await io.awaitInitialised;

        for (int i = 0; i < m_slots.Length; i++)
        {
            if (io.saveSlots[i] == null)
                m_slots[i].GetComponentInChildren<TextMeshProUGUI>().text = "New Game"; //#todo #jack #matthew add in time played data
            else
                m_slots[i].GetComponentInChildren<TextMeshProUGUI>().text = "Save " + (i + 1); //#todo #jack #matthew add in time played data
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public async void OnSave(int slotNumber)
    {
        if (io.saveSlots[slotNumber] == null)
            await io.CreateNewSave(slotNumber, true);
        else
        {
            await io.LoadSaveAsync(io.saveSlots[slotNumber]); //#todo #matthew cleany up please
        }
    }

    public void OnDelete(int slotNumber)
    {
        if (io.saveSlots[slotNumber] != null)
        {
            System.IO.File.Delete(io.saveSlots[slotNumber].m_filepath);
            io.LoadSaveSlots(); //#todo #matthew you better fix
            GetComponentInParent<CanvasTool.ButtonWrapper>().buttons[slotNumber].sceneName = LevelModule.ResolveSceneNameString(blu.LevelID._default);
            OnOpen();
        }
    }
}