using System;
using UnityEngine;

namespace HexFiled
{
    [Serializable]
    public struct CellColor
    {
        [SerializeField] private UnitColor _unitColor;
        [SerializeField] private Texture _texture;
        [SerializeField] private GameObject _vfxCellCapturePrefab;
        [SerializeField] private GameObject _vfxDeathPrefab;
        

        public UnitColor UnitColor => _unitColor;
        public Texture Texture => _texture;
        public GameObject VFXDeathPrefab => _vfxDeathPrefab;
        public GameObject VFXCellCapturePrefab => _vfxCellCapturePrefab;
        
        

        public bool Equals(CellColor obj)
        {
            return obj._unitColor == _unitColor;
        }
    }
}