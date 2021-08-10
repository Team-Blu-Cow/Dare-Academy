using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotButton : MonoBehaviour
{
    [SerializeField] public List<SaveSlotButtonObject> m_buttonObjects;

    [SerializeField, HideInInspector] public Button button;

    [SerializeField] public TextMeshProUGUI saveName;
    [SerializeField] public TextMeshProUGUI playTime;

    [SerializeField] public GameObject hearts;
    [SerializeField] public GameObject heartPrefab;
    [SerializeField] public GameObject energy;
    [SerializeField] public GameObject energyPrefab;

    [SerializeField] public Image[] powerupIcons;
    [SerializeField] public bool[] powerupUnlocked;

    private Color noPowerupColour = new Color(0.1f, 0.1f, 0.1f, 1);

    public bool isInitialised = false;

    private void OnValidate()
    {
        button = GetComponent<Button>();
        if (m_buttonObjects != null)
        {
            foreach (var obj in m_buttonObjects)
            {
                obj.button = button;
                obj.noPowerupColour = noPowerupColour;
            }
        }
        powerupUnlocked = new bool[3] { false,false,false};
    }

    private void Awake()
    {
        isInitialised = false;
    }

    public void SetSelected(bool isSelected)
    {
        foreach(var obj in m_buttonObjects)
        {
            obj.SetColour(isSelected);
        }
    }

    public void SpawnHearts(int num)
    {
        if (isInitialised)
            return;
        for (int i = 0; i < num; i++)
        {
            GameObject go = Instantiate(heartPrefab, hearts.transform);
            m_buttonObjects.Add(new SaveSlotButtonObject(go, button, SaveSlotButtonObject.Type.IMAGE));
        }
    }

    public void SpawnEnergy(int num)
    {
        if (isInitialised)
            return;

        for (int i = 0; i < num; i++)
        {
            GameObject go = Instantiate(energyPrefab, energy.transform);
            m_buttonObjects.Add(new SaveSlotButtonObject(go, button, SaveSlotButtonObject.Type.IMAGE));
        }
    }

    public void InitPowerups()
    {
        if (isInitialised)
            return;

        for (int i = 0; i < 3; i++)
        {
            SaveSlotButtonObject ssbo = new SaveSlotButtonObject();

            ssbo.gameObject = powerupIcons[i].gameObject;
            ssbo.button = button;
            ssbo.unlocked = powerupUnlocked[i];
            ssbo.noPowerupColour = noPowerupColour;
            ssbo.overrideColour = true;
            ssbo.type = SaveSlotButtonObject.Type.POWERUPICON;

            m_buttonObjects.Add(ssbo);

            bool isSelected = (i == 0);

            ssbo.SetColour(isSelected);
        }
    }

    public void DisableExtras()
    {
        hearts.SetActive(false);
        energy.SetActive(false);
        playTime.gameObject.SetActive(false);
        powerupIcons[0].gameObject.SetActive(false);
        powerupIcons[1].gameObject.SetActive(false);
        powerupIcons[2].gameObject.SetActive(false);
    }

    public void EnableExtras()
    {
        hearts.SetActive(true);
        energy.SetActive(true);
        playTime.gameObject.SetActive(true);
        powerupIcons[0].gameObject.SetActive(true);
        powerupIcons[1].gameObject.SetActive(true);
        powerupIcons[2].gameObject.SetActive(true);
    }

}

[System.Serializable]
public class SaveSlotButtonObject
{
    public GameObject gameObject;

    public bool overrideColour = true;

    public Button button;

    public bool unlocked;

    [HideInInspector]public Color noPowerupColour;

    public enum Type
    {
        IMAGE,
        TEXT,
        POWERUPICON
    }

    public Type type;

    public SaveSlotButtonObject()
    {
        gameObject      = null;
        button          = null;
        type            = Type.TEXT;
        overrideColour  = true;
    }

    public SaveSlotButtonObject(GameObject _gameObject, Button _button, Type _type, bool _overrideColour = true)
    {
        gameObject      = _gameObject;
        button          = _button;
        type            = _type;
        overrideColour  = _overrideColour;
    }

    public void SetColour(bool isSelected)
    {
        switch (type)
        {
            case Type.IMAGE:
                if (isSelected)
                    gameObject.GetComponent<Image>().color = button.colors.selectedColor;
                else
                    gameObject.GetComponent<Image>().color = button.colors.normalColor;
                break;

            case Type.TEXT:

                if (isSelected)
                    gameObject.GetComponent<TextMeshProUGUI>().color = button.colors.selectedColor;
                else
                {
                    if(unlocked)
                        gameObject.GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 0);
                    else
                        gameObject.GetComponent<TextMeshProUGUI>().color = button.colors.normalColor;
                }
                break;

            case Type.POWERUPICON:
                if(unlocked)
                {
                    if (isSelected)
                        gameObject.GetComponent<Image>().color = button.colors.selectedColor;
                    else
                        gameObject.GetComponent<Image>().color = button.colors.normalColor;
                }
                else
                {
                    if (isSelected)
                        gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    else
                        gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                }
                break;
        }


    }

}

