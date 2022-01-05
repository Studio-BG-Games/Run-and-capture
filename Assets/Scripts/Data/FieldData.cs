using System.Collections.Generic;
using HexFiled;
using TMPro;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "FieldData", menuName = "Data/Field Data")]
    public class FieldData : ScriptableObject
    {
        public int width = 6;
        public int height = 6;
        public int hexCaptureManaCost;

        public GameObject cellPrefab;
        public TMP_Text cellLabelPrefab;
        public GameObject CoordinatesCanvas;
        public List<CellColor> colors;
    }
}