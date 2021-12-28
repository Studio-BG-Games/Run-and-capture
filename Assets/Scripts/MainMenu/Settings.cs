using System;
using DefaultNamespace;

namespace MainMenu
{
    [Serializable]
    public class Settings
    {
        public bool isMusicAllowed;
        public bool isSFXAllowed;

        public Settings(GameMenuData data)
        {
            isMusicAllowed = data.isMusicAllowed;
            isSFXAllowed = data.isMusicAllowed;
        }
    }
}