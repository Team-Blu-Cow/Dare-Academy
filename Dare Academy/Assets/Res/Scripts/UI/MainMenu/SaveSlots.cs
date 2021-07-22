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

        foreach (var button in GetComponentInParent<CanvasTool.ButtonWrapper>().buttons)
        {
            button.sceneName = "Crashsite Top"; //#todo #matthew Change to saved scene
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
        System.IO.File.Delete(io.saveSlots[slotNumber].m_filepath);
        io.LoadSaveSlots(); //#todo #matthew you better fix

        OnOpen();
    }
}