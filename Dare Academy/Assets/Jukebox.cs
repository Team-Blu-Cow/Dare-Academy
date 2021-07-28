using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// #TODO #Anyone cleanup you todos cucks, half that shit is already fixed and I have to sift through it

namespace blu
{
    public enum MusicParameter
    {
        InCombat = 0,
        Muffled = 1
    }

    public class Jukebox : MonoBehaviour
    {
        [FMODUnity.EventRef]
        public string StartSong = "";

        private void OnEnable()
        {
            if (StartSong != "")
            {
                if (StartSong == "NONE")
                    Stop();
                else
                    Play(StartSong);
            }
        }

        public void Play(string in_name)
        {
            if (blu.App.GetModule<AudioModule>().GetCurrentSong() != null)
            {
                if (blu.App.GetModule<AudioModule>().GetCurrentSong().name == in_name)
                    return;
                blu.App.GetModule<AudioModule>().StopMusicEvent(blu.App.GetModule<AudioModule>().GetCurrentSong().name, true);
            }

            blu.App.GetModule<AudioModule>().PlayMusicEvent(in_name);
        }

        public void Pause()
        {
            blu.App.GetModule<AudioModule>().PauseMusicEvent(blu.App.GetModule<AudioModule>().GetCurrentSong().name);
        }

        public void Resume()
        {
            blu.App.GetModule<AudioModule>().UnpauseMusicEvent(blu.App.GetModule<AudioModule>().GetCurrentSong().name);
        }

        public void Stop()
        {
            if (blu.App.GetModule<AudioModule>().GetCurrentSong() != null)
            {
                blu.App.GetModule<AudioModule>().StopMusicEvent(blu.App.GetModule<AudioModule>().GetCurrentSong().name);
            }
        }

        public void SetParameter(MusicParameter in_parameter, float in_value)
        {
            if (blu.App.GetModule<AudioModule>().GetCurrentSong() == null)
                return;

            switch (in_parameter)
            {
                case MusicParameter.InCombat:
                    blu.App.GetModule<AudioModule>().GetCurrentSong().SetParameter("InCombat", in_value);
                    break;

                case MusicParameter.Muffled:
                    blu.App.GetModule<AudioModule>().GetCurrentSong().SetParameter("Muffled", in_value);
                    break;
            }
        }
    }
}