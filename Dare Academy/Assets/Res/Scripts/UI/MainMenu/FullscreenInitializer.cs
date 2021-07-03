using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using blu;

public class FullscreenInitializer : MonoBehaviour
{
    private Toggle _fullscreenToggle;

    private void OnEnable()
    {
        _fullscreenToggle = GetComponent<Toggle>();
        _fullscreenToggle.isOn = App.GetModule<SettingsModule>().m_graphicsSettings.Fullscreen;
    }

    public void ToggleFullscreen(bool in_value)
    {
        Resolution _currentResolution = App.GetModule<SettingsModule>().m_graphicsSettings.Resolution;
        Screen.SetResolution(_currentResolution.width, _currentResolution.height, in_value, _currentResolution.refreshRate);
        App.GetModule<SettingsModule>().m_graphicsSettings.Fullscreen = in_value;
        SettingsModule.Save();
    }
}