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

        public GameObject PlayEffect(GameObject effect, Vector3 pos)
        {
            var obj = Object.Instantiate(effect, pos, effect.transform.rotation);
            obj.AddComponent<VFXView>();
            return obj;
        }
        
        public GameObject PlayEffect(GameObject effect, Transform pos)
        {
            var obj = Object.Instantiate(effect, pos.position, pos.rotation);
            obj.AddComponent<VFXView>();
            return obj;
        }
    }
}