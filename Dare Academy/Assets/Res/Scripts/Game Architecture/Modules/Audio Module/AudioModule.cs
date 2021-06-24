using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace blu
{
    public class AudioModule : Module
    {
        // sets up two caches of event memory to avoid delay when audio is player
        // NOTE: only use this module to play sounds for mono events like one shots
        //       spacial audio should be done inside of the editor using FMOD event imitters

        private Dictionary<string, AudioEvent> _musicEvents = new Dictionary<string, AudioEvent>();
        private Dictionary<string, AudioEvent> _audioEvents = new Dictionary<string, AudioEvent>();

        public void NewAudioEvent(string name, int poly = 0) // use "object/event"
        {                                      // e.g. "player/footstep"
            if (name != null)
            {
                if (poly == 0)
                    _audioEvents.Add(name, new AudioEvent(name));
                else
                    _audioEvents.Add(name, new AudioEvent(name, poly));
            }
            else
            {
                _audioEvents.Add(null, new AudioEvent());
            }
        }

        public void NewMusicEvent(string name) // use "object/event"
        {                                      // e.g. "player/footstep"
            if (name != null)
            {
                _musicEvents.Add(name, new AudioEvent(name));
            }
            else
            {
                _musicEvents.Add(name, new AudioEvent());
            }
        }

        public void TogglePauseMusicEvent(string name)
        {
            _musicEvents[name].TogglePause();
        }

        public void PauseMusicEvent(string name)
        {
            _musicEvents[name].Pause();
        }

        public void UnpauseMusicEvent(string name)
        {
            _musicEvents[name].Unpause();
        }

        public void StopMusicEvent(string name, bool fade = false)
        {
            if (fade)

                _musicEvents[name].FadeStop();
            else
                _musicEvents[name].HardStop();
        }

        public void PlayAudioEvent(string name) // use copied path from event browser
        {                                       // e.g. "event:/player/footstep"
            _audioEvents[name].Play();
        }

        public void PlayMusicEvent(string name) // use copied path from event browser
        {                                       // e.g. "event:/player/footstep"
            _musicEvents[name].Play();
        }

        public void DeleteAudioEvent(string name)
        {
            _audioEvents[name].DeleteEvent();
        }

        public void DeleteMusicEvent(string name)
        {
            _musicEvents[name].DeleteEvent();
        }

        public AudioEvent GetAudioEvent(string name)
        {
            return _audioEvents[name];
        }

        public AudioEvent GetMusicEvent(string name)
        {
            return _musicEvents[name];
        }

        public void StopAllEvents(bool fade = false)
        {
            if (fade)
            {
                FMODUnity.RuntimeManager.GetBus("bus:/Master").stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
            else
            {
                FMODUnity.RuntimeManager.GetBus("bus:/Master").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }
        }

        public void StopAllMusicEvents(bool fade = false)
        {
            if (fade)
            {
                FMODUnity.RuntimeManager.GetBus("bus:/Master/Music").stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
            else
            {
                FMODUnity.RuntimeManager.GetBus("bus:/Master/Music").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }
        }

        public void StopAllSFXEvents(bool fade = false)
        {
            if (fade)
            {
                FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX").stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
            else
            {
                FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }
        }

        public override void Initialize()
        {
            Debug.Log("[App]: Initializing audio module");
            CreateEvents();
        }

        private void CreateEvents()
        {
            // put any new mono event in here
        }
    }
}