using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using blu;

public class ResolutionDropdown : MonoBehaviour
{
    private TMPro.TMP_Dropdown _dropdown;
    private Resolution[] resolutions;

    private List<Resolution> listResolutions;
    private ScrollRect _scrollRect;
    private int maxValues = 0;

    private void Awake()
    {
        _scrollRect = GetComponentInChildren<ScrollRect>(true);
        _dropdown = GetComponent<TMP_Dropdown>();
        _dropdown.options.Clear();
        resolutions = Screen.resolutions;
        listResolutions = new List<Resolution>(resolutions);
        List<string> acceptedRes = new List<string>();
        List<Resolution> unacceptableRes = new List<Resolution>();
        foreach (Resolution res in listResolutions)
        {
            if (res.width % 16 == 0 && res.height % 9 == 0)
            {
                acceptedRes.Add(res.ToString());
            }
            else
            {
                unacceptableRes.Add(res);
            }
        }

        foreach (Resolution badRes in unacceptableRes)
        {
            listResolutions.Remove(badRes);
        }
        maxValues = acceptedRes.Count;
        _dropdown.AddOptions(acceptedRes);
        for (int i = 0; i < listResolutions.Count; i++)
        {
            if (listResolutions[i].Equals(App.GetModule<SettingsModule>().m_graphicsSettings.Resolution))
            {
                _dropdown.value = i;
                break;
            }
        }
    }

    public void HandleInput(int in_value)
    {
        Resolution newResolution = listResolutions[in_value];
        Application.targetFrameRate = newResolution.refreshRate;
        Screen.SetResolution(newResolution.width, newResolution.height, App.GetModule<SettingsModule>().m_graphicsSettings.Fullscreen, newResolution.refreshRate);
        App.GetModule<SettingsModule>().m_graphicsSettings.Resolution = newResolution;
        SettingsModule.Save();
    }
}