using System.IO;
using Runtime.Data;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "Data/Data")]
    public class Data : ScriptableObject
    {
        [SerializeField] private string fieldDataPath;
        private FieldData _fieldData;
        
        public FieldData Field
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

       
        
        private static T Load<T>(string resourcesPath) where T : Object =>
            Resources.Load<T>(Path.ChangeExtension(resourcesPath, null));
    }
}