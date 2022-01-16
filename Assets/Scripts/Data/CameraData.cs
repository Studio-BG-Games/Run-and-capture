using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "CameraData", menuName = "Data/Camera Data")]
    public class CameraData : ScriptableObject
    {
        public Vector3 offset;
        public float smoothSpeed;
    }
}