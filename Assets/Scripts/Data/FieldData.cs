using System.Collections.Generic;
using HexFiled;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "FieldData", menuName = "Data/Field Data")]
    public class FieldData : SerializedScriptableObject
    {
        public int width = 6;
        public int height = 6;
        public int hexCaptureManaCost;
        public int hexHardCaptureManaCost;
        public float hexHardCaptureTime;
        public GameObject cellPrefab;
        public TMP_Text cellLabelPrefab;
        public GameObject CoordinatesCanvas;
        public Dictionary<UnitColor, CellColor> colors;
    }
}