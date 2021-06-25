using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using blu;

public class MuteAudio : MonoBehaviour
{
    private Toggle _mute;

    private void OnEnable()
    {
        _mute = GetComponent<Toggle>();

        _mute.isOn = App.GetModule<SettingsModule>().audioSettings.muteAudio;
    }

    public void ToggleMuteAudio(bool in_value)
    {
        App.GetModule<SettingsModule>().audioSettings.muteAudio = in_value;
        SettingsModule.Save();
    }
}