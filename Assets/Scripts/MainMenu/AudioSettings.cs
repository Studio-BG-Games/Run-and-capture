using System;
using DefaultNamespace;

namespace MainMenu
{
    [Serializable]
    public class AudioSettings
    {
        public float musicVolume;
        public float sfxVolume;

        public AudioSettings(float musicVolume, float sfxVolume)
        {
            this.musicVolume = musicVolume;
            this.sfxVolume = sfxVolume;
        }
    }
}