using System.IO;
using UnityEditor;
using UnityEngine;
using Weapons;

namespace Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "Data/Data")]
    public class Data : ScriptableObject
    {
       

        public string levelName;
        [SerializeField] private string fieldDataPath = "FieldData";
        private FieldData _fieldData;
        [SerializeField] private string cameraDataPath = "CameraData";
        private CameraData _cameraData;
        [SerializeField] private string unitDataPath = "UnitData";
        private UnitData _unitData;
        [SerializeField] private string weaponDataPath = "WeaponsData";
        private WeaponsData _weaponData;
        [SerializeField] private string uiDataPath = "UIData";
        private UIData _uiData;
        [SerializeField] private string musicDataPath = "MusicData";
        private MusicData _musicData;
        [SerializeField] private string itemDataPath = "ItemData";
        private ItemsData _itemsData;
        [SerializeField] private string aiDataPath = "AIData";
        private AIData _aiData;
        [SerializeField] private string wariorsDataPath = "WariorsData";
        private WariorsData _wariorsData;

        [SerializeField] private string chosenWeaponDataPath = "ChosenWeapon.json";

        private string pathToLevel => "Data/" + levelName + "/";

        public Weapon ChosenWeapon => WeaponsData.WeaponsList[int.Parse(File.ReadAllText(Application.persistentDataPath + "/" + chosenWeaponDataPath))];

        public WariorsData WariorsData
        {
            get
            {
                if (_wariorsData == null)
                {
                    _wariorsData = Load<WariorsData>(pathToLevel + wariorsDataPath);
                }

                return _wariorsData;
            }
        }

        public AIData AIData
        {
            get
            {
                if (_aiData == null)
                {
                    _aiData = Load<AIData>(pathToLevel + aiDataPath);
                }

                return _aiData;
            }
        }

        public ItemsData ItemsData
        {
            get
            {
                if (_itemsData == null)
                {
                    _itemsData = Load<ItemsData>(pathToLevel + itemDataPath);
                }

                return _itemsData;
            }
        }

        public MusicData MusicData
        {
            get
            {
                if (_musicData == null)
                {
                    _musicData = Load<MusicData>(pathToLevel + musicDataPath);
                }

                return _musicData;
            }
        }

        public UIData UIData
        {
            get
            {
                if (_uiData == null)
                {
                    _uiData = Load<UIData>(pathToLevel + uiDataPath);
                }

                return _uiData;
            }
        }


        public WeaponsData WeaponsData
        {
            get
            {
                if (_weaponData == null)
                {
                    _weaponData = Load<WeaponsData>(pathToLevel + weaponDataPath);
                }

                return _weaponData;
            }
        }

        public FieldData FieldData
        {
            get
            {
                if (_fieldData == null)
                {
                    _fieldData = Load<FieldData>(pathToLevel + fieldDataPath);
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
                    _cameraData = Load<CameraData>(pathToLevel + cameraDataPath);
                }

                return _cameraData;
            }
        }

        public UnitData UnitData
        {
            get
            {
                if (_unitData == null)
                {
                    _unitData = Load<UnitData>(pathToLevel + unitDataPath);
                }

                return _unitData;
            }
        }
        

        private static T Load<T>(string resourcesPath) where T : Object =>
            Resources.Load<T>(Path.ChangeExtension(resourcesPath, null));
    }
}