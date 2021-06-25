using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using blu;

public class SliderInitializer : MonoBehaviour
{
    private enum SliderType
    {
        NONE = 0,
        MasterBus,
        SFXBus,
        MusicBus
    }

    private Slider _slider;
    [SerializeField] private SliderType _type;

    private void OnEnable()
    {
        _slider = GetComponent<Slider>();

        switch (_type)
        {
            case SliderType.MasterBus:
                _slider.value = App.GetModule<SettingsModule>().audioSettings.masterVolume;
                break;

            case SliderType.SFXBus:
                _slider.value = App.GetModule<SettingsModule>().audioSettings.sfxVolume;
                break;

            case SliderType.MusicBus:
                _slider.value = App.GetModule<SettingsModule>().audioSettings.musicVolume;
                break;

            default:
                break;
        }
    }

    public void SetValue(float in_value)
    {
        switch (_type)
        {
            case SliderType.MasterBus:
                App.GetModule<SettingsModule>().audioSettings.masterVolume = in_value;
                break;

            case SliderType.SFXBus:
                App.GetModule<SettingsModule>().audioSettings.sfxVolume = in_value;
                break;

            case SliderType.MusicBus:
                App.GetModule<SettingsModule>().audioSettings.musicVolume = in_value;
                break;

            default:
                break;
        }
        SettingsModule.Save();
    }
}