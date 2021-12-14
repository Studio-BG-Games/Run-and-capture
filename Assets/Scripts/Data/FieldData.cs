using HexFiled;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Data
{
    [CreateAssetMenu(fileName = "FieldData", menuName = "Data/Field Data")]
    public class FieldData : ScriptableObject
    {
        public int width = 6;
        public int height = 6;
        public Color defaultColor = Color.white;
        public Color touchedColor = Color.magenta;

        public HexCell cellPrefab;
        public Text cellLabelPrefab;
        public GameObject CoordinatesCanvas;
        public GameObject HexMeshPrefab;
    }
}