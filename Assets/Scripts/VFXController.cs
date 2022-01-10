using Data;
using UnityEngine;

namespace DefaultNamespace
{
    public class VFXController
    {
        private static VFXController _instance;
        public static VFXController Instance
        {
            get => _instance;
            private set => _instance = value;
        }

        private VFXData _data;
        public VFXData Data => _data;

        public VFXController(VFXData data)
        {
            Instance ??= this;
            _data = data;
        }
    }
}