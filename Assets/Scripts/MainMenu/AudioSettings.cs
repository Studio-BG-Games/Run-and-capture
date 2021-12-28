using System;
using DefaultNamespace;

namespace MainMenu
{
    [Serializable]
    public class AudioSettings
    {
        public bool isMusicAllowed;
        public bool isSFXAllowed;

        public AudioSettings(GameMenuData data)
        {
            isMusicAllowed = data.isMusicAllowed;
            isSFXAllowed = data.isMusicAllowed;
        }
    }
}