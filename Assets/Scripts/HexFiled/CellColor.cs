using System;
using UnityEngine;

namespace HexFiled
{
    [Serializable]
    public struct CellColor
    {
        [SerializeField] private Texture _texture;
        [SerializeField] private GameObject _vfxCellCapturePrefab;
        [SerializeField] private GameObject _vfxDeathPrefab;
        

       
        public Texture Texture => _texture;
        public GameObject VFXDeathPrefab => _vfxDeathPrefab;
        public GameObject VFXCellCapturePrefab => _vfxCellCapturePrefab;
        
        
    }
}