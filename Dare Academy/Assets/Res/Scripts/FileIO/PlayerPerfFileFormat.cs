using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Threading.Tasks;

namespace blu.FileIO
{
    public abstract class PlayerPerfFileFormat

    {
        public static void Save()
        {
            PlayerPrefs.Save();
        }

        public abstract bool Init();
    }

    public class AudioSettings : PlayerPerfFileFormat
    {
        private FMOD.Studio.Bus _masterBus;
        private FMOD.Studio.Bus _SFXBus;
        private FMOD.Studio.Bus _musicBus;
        private bool _muted;

        public FMOD.Studio.Bus MasterBus
        { get => _masterBus; }

        public FMOD.Studio.Bus SFXBus
        { get => _SFXBus; }

        public FMOD.Studio.Bus MusicBus
        { get => _musicBus; }

        public override bool Init()
        {
            try
            {
                _muted = PlayerPrefs.GetInt("AudioMuted", 0) == 1;
            }
            catch (System.Exception ex)
            { Debug.LogWarning(ex); }
            try
            {
                _masterBus = FMODUnity.RuntimeManager.GetBus("bus:/Master");
                _masterBus.setVolume(PlayerPrefs.GetFloat("MasterVolume", 1f));
                _masterBus.setMute(_muted);
            }
            catch (System.Exception ex)
            { Debug.LogWarning(ex); }

            try
            {
                _SFXBus = FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX");
                _SFXBus.setVolume(PlayerPrefs.GetFloat("SFXVolume", 0.5f));
            }
            catch (System.Exception ex)
            { Debug.LogWarning(ex); }

            try
            {
                _musicBus = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
                _musicBus.setVolume(PlayerPrefs.GetFloat("MusicVolume", 0.5f));
            }
            catch (System.Exception ex)
            { Debug.LogWarning(ex); }

            return true;
        }

        public float masterVolume
        {
            get
            {
                try
                {
                    _masterBus.getVolume(out float out_volume);
                    return out_volume;
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning(ex);
                    return 0f;
                }
            }
            set
            {
                try
                {
                    _masterBus.setVolume(value);
                    PlayerPrefs.SetFloat("MasterVolume", value);
                }
                catch (System.Exception ex)
                { Debug.LogWarning(ex); }
            }
        }

        public float sfxVolume
        {
            get
            {
                try
                {
                    _SFXBus.getVolume(out float out_volume);
                    return out_volume;
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning(ex);
                    return 0f;
                }
            }
            set
            {
                try
                {
                    PlayerPrefs.SetFloat("SFXVolume", value);
                    _SFXBus.setVolume(value);
                }
                catch (System.Exception ex)
                { Debug.LogWarning(ex); }
            }
        }

        public float musicVolume
        {
            get
            {
                try
                {
                    _musicBus.getVolume(out float out_volume);
                    return out_volume;
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning(ex);
                    return 0f;
                }
            }
            set
            {
                try
                {
                    PlayerPrefs.SetFloat("MusicVolume", value);
                    _musicBus.setVolume(value);
                }
                catch (System.Exception ex)
                { Debug.LogWarning(ex); }
            }
        }

        public bool muteAudio
        {
            get
            {
                try
                {
                    return _muted;
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning(ex);
                    return false;
                }
            }
            set
            {
                try
                {
                    _muted = value;
                    PlayerPrefs.SetInt("AudioMuted", _muted ? 1 : 0);
                    _masterBus.setMute(_muted);
                }
                catch (System.Exception ex)
                { Debug.LogWarning(ex); }
            }
        }
    }

    public class GraphicsSettings : PlayerPerfFileFormat
    {
        public enum MSAASettings
        {
            NONE = 0,
            TWOX = 2,
            FOURX = 4,
            EIGHTX = 8,
        }

        //private UniversalRenderPipelineAsset _URPAsset;

        private Resolution _currentResolution = new Resolution();

        public Resolution Resolution
        {
            get => _currentResolution;
            set
            {
                _currentResolution = value;
                PlayerPrefs.SetInt("RefreshRate", _currentResolution.refreshRate);
                PlayerPrefs.SetInt("ScreenWidth", _currentResolution.width);
                PlayerPrefs.SetInt("ScreenHeight", _currentResolution.height);
            }
        }

        private bool _fullscreen = false;

        public bool Fullscreen
        {
            get => _fullscreen;
            set
            {
                _fullscreen = value;
                PlayerPrefs.SetInt("Fullscreen", _fullscreen ? 1 : 0);
            }
        }

        public override bool Init()
        {
            _currentResolution.refreshRate = PlayerPrefs.GetInt("RefreshRate", 60);
            _currentResolution.width = PlayerPrefs.GetInt("ScreenWidth", 1600);
            _currentResolution.height = PlayerPrefs.GetInt("ScreenHeight", 900);
            _fullscreen = PlayerPrefs.GetInt("Fullscreen", 0) == 1;

            Application.targetFrameRate = _currentResolution.refreshRate;
            Screen.SetResolution(_currentResolution.width, _currentResolution.height, _fullscreen, _currentResolution.refreshRate);

            return true;
        }
    }
}