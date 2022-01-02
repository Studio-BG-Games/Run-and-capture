using System;
using DefaultNamespace;

namespace MainMenu
{
    [Serializable]
    public class AudioSettings
    {
        public float musicVolume;
        public float sfxVolume;

        public AudioSettings(GameMenuData data)
        {
            musicVolume = data.musicVolume;
            sfxVolume = data.sfxVolume;
        }
    }
}