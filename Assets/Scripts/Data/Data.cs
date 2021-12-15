using System.IO;
using Chars;
using Runtime.Data;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "Data/Data")]
    public class Data : ScriptableObject
    {
        [SerializeField] private string fieldDataPath;
        private FieldData _fieldData;
        [SerializeField] private string cameraDataPath;
        private CameraData _cameraData;
        [SerializeField] private string playerDataPath;
        private PlayerData _playerData;

        public FieldData FieldData
        {
            get
            {
                if (_fieldData == null)
                {
                    _fieldData = Load<FieldData>("Data/" + fieldDataPath);
                }

                return _fieldData;
            }
        }

        public CameraData CameraData
        {
            get
            {
                if (_cameraData == null)
                {
                    _cameraData = Load<CameraData>("Data/" + cameraDataPath);
                }

                return _cameraData;
            }
        }

        public PlayerData PlayerData
        {
            get
            {
                if (_playerData == null)
                {
                    _playerData = Load<PlayerData>("Data/" + playerDataPath);
                }

                return _playerData;
            }
        }


        private static T Load<T>(string resourcesPath) where T : Object =>
            Resources.Load<T>(Path.ChangeExtension(resourcesPath, null));
    }
}