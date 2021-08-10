using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace blu
{
    public class SettingsModule : Module
    {
        public FileIO.GraphicsSettings m_graphicsSettings = new FileIO.GraphicsSettings();
        public FileIO.AudioSettings audioSettings = new FileIO.AudioSettings();

        protected override void SetDependancies()
        {
            _dependancies.Add(typeof(AudioModule));
        }

        public override void Initialize()
        {
            Debug.Log("[App]: Initializing settings module");
            audioSettings.Init();
            m_graphicsSettings.Init();

            return;
        }

        public static void Save() => PlayerPrefs.Save();
    }
}