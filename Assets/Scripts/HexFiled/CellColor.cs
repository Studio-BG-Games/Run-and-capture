using System;
using UnityEngine;

namespace HexFiled
{
    [Serializable]
    public struct CellColor
    {
        [SerializeField] private UnitColor _unitColor;
        [SerializeField] private Texture _texture;
        [SerializeField] private GameObject _vfxPrefab;
        [SerializeField] private Texture _buildinTexture;

        public UnitColor UnitColor => _unitColor;
        public Texture Texture => _texture;
        public Texture BuildingTexture => _buildinTexture;
        public GameObject VFXPrefab => _vfxPrefab;
        
        

        public bool Equals(CellColor obj)
        {
            return obj._unitColor == _unitColor;
        }
    }
}