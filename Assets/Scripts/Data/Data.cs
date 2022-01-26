﻿using System.IO;
using DefaultNamespace.Weapons;
using UnityEngine;
using Weapons;

namespace Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "Data/Data")]
    public class Data : ScriptableObject
    {
        [SerializeField] private string fieldDataPath;
        private FieldData _fieldData;
        [SerializeField] private string cameraDataPath;
        private CameraData _cameraData;
        [SerializeField] private string unitDataPath;
        private UnitData _unitData;
        [SerializeField] private string weaponDataPath;
        private WeaponsData _weaponData;
        [SerializeField] private string uiDataPath;
        private UIData _uiData;
        [SerializeField] private string musicDataPath;
        private MusicData _musicData;
        [SerializeField] private string itemDataPath;
        private ItemsData _itemsData;
        [SerializeField] private string aiDataPath;
        private AIData _aiData;
        [SerializeField] private string chosenWeaponDataPath;

        public Weapon ChosenWeapon =>
            JsonUtility.FromJson<Weapon>(File.ReadAllText(Application.persistentDataPath + "/" + chosenWeaponDataPath));

        public AIData AIData
        {
            get
            {
                if (_aiData == null)
                {
                    _aiData = Load<AIData>("Data/" + aiDataPath);
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
                    _itemsData = Load<ItemsData>("Data/" + itemDataPath);
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
                    _musicData = Load<MusicData>("Data/" + musicDataPath);
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
                    _uiData = Load<UIData>("Data/" + uiDataPath);
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
                    _weaponData = Load<WeaponsData>("Data/" + weaponDataPath);
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

        public UnitData UnitData
        {
            get
            {
                if (_unitData == null)
                {
                    _unitData = Load<UnitData>("Data/" + unitDataPath);
                }

                return _unitData;
            }
        }


        private static T Load<T>(string resourcesPath) where T : Object =>
            Resources.Load<T>(Path.ChangeExtension(resourcesPath, null));
    }
}