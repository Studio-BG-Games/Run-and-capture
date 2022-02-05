using System.Collections.Generic;
using System.IO;
using HexFiled;
using UnityEngine;
using UnityEngine.Purchasing.MiniJSON;

namespace DefaultNamespace
{
    public class HexMapEditor : MonoBehaviour
    {
        
        [SerializeField] private GameObject hexPrefab;
        [SerializeField] private GameObject labelPrefab;

        private HexGrid hexGrid;

        private Color activeColor;
        private GameObject fieldBaseGameObject;


        [EditorButton]
        private void DrawMap(int x, int y)
        {
            if (hexGrid != null)
            {
                Destroy(fieldBaseGameObject);
            }

            
            WriteToBinaryFile("HexField", hexGrid);
            fieldBaseGameObject = hexGrid.SpawnField();
        }

        [EditorButton]
        private void LoadMap(string name)
        {
            if (File.Exists(name))
            {
                hexGrid = ReadFromBinaryFile<HexGrid>("HexField");
                hexGrid.SpawnField();
            }
        }
        
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

       
        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                HandleInput();
            }
        }

        void HandleInput()
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                var coord = HexCoordinates.FromPosition(hit.point);
                hexGrid.GetCellFromCoord(coord).PaintHex(UnitColor.Green);
            }
        }
    }
}