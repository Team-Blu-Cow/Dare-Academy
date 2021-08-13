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
        public string currentSong;

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

        public void NewMusicEvent(string name)
        {
            if (name != null)
            {
                _musicEvents.Add(name, new AudioEvent(name));
            }
            else
            {
                _musicEvents.Add(name, new AudioEvent());
                Debug.LogWarning("Invalid Name");
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
            currentSong = "";
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
            currentSong = name;
        }

        public AudioEvent GetCurrentSong()
        {
            if (currentSong != "")
            {
                return _musicEvents[currentSong];
            }
            return null;
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
            currentSong = "";
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
            NewMusicEvent("event:/Music/Crash Site/Crash Site");
            NewMusicEvent("event:/Music/Main Menu/Main Theme");
            NewMusicEvent("event:/Music/Mushroom Forest/Boyfriends Helmet");
            NewMusicEvent("event:/Music/Mushroom Forest/Undergrowth");
            NewAudioEvent("event:/SFX/Player/sfx_ability_select");
            NewAudioEvent("event:/SFX/Player/sfx_dash_chrage");
            NewAudioEvent("event:/SFX/Player/sfx_dash_release");
            NewAudioEvent("event:/SFX/Player/sfx_ability_fail");
            NewAudioEvent("event:/SFX/Systems/sfx_quest_accepted");
            NewAudioEvent("event:/SFX/Systems/sfx_return_main_menu");
            NewAudioEvent("event:/SFX/UI/Pointer Enter");
            NewAudioEvent("event:/SFX/UI/sfx_unpause");
            NewAudioEvent("event:/SFX/UI/sfx_pause");
            NewAudioEvent("event:/SFX/Player/sfx_dash");
            NewAudioEvent("event:/SFX/Player/sfx_player_hit");
            NewAudioEvent("event:/SFX/melee pathfinder/hammy/sfx_melee_munch");
            NewAudioEvent("event:/SFX/melee pathfinder/truffle/sfx_truffle_hit");
            NewAudioEvent("event:/SFX/archer/sfx_fly_death");
            NewAudioEvent("event:/SFX/archer/sfx_enemy_bullet_fly");
            NewAudioEvent("event:/SFX/8dir/sfx_eight_death");
            NewAudioEvent("event:/SFX/8dir/sfx_enemy_bullet_eight");
            NewAudioEvent("event:/SFX/Player/sfx_footstep", 15);
            NewAudioEvent("event:/SFX/Player/sfx_shoot", 15);
            NewAudioEvent("event:/SFX/Systems/dialogue/sfx_dialogue_blip", 15);
            NewAudioEvent("event:/SFX/UI/sfx_energy_pickup", 15);
            NewAudioEvent("event:/SFX/UI/sfx_heart_pickup", 15);
            NewAudioEvent("event:/SFX/UI/sfx_save_game", 15);
            NewAudioEvent("event:/SFX/UI/Vitalsfx_ability_left", 15);
            NewAudioEvent("event:/SFX/UI/Vitalsfx_ability_right", 15);
            NewAudioEvent("event:/SFX/sfx_kitty", 15);
            // put any new mono event in here
        }
    }
}