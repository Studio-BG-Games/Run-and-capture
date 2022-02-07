using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using HexFiled;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing.MiniJSON;
using Object = UnityEngine.Object;

namespace DefaultNamespace
{
    [Serializable]
    public class GridToSave
    {
        public SerializibleHexCell[] cells;
        public int height;
        public int width;
    }

    [Serializable]
    public class SerializibleHexCell
    {
        public HexCoordinates HexCoordinates;
        public int index;
    }


    public class HexMapEditor : MonoBehaviour
    {
        [SerializeField] private GameObject hexPrefab;
        [SerializeField] private TMP_Text labelPrefab;
        [SerializeField] private GameObject gridCanvas;

        private GameObject gridCanvasInstance;
        private HexCell[] _cells;
        private HexGrid hexGrid;
        private int _width;
        private int _height;

        private Color activeColor;
        private GameObject fieldBaseGameObject;
        private GameObject labelCanvas;


        [EditorButton]
        private void DrawMap(int x, int y)
        {
            if (fieldBaseGameObject != null)
            {
                DestroyImmediate(fieldBaseGameObject);
            }

            if (gridCanvasInstance == null)
            {
                DestroyImmediate(gridCanvasInstance);
            }

            gridCanvasInstance = Instantiate(gridCanvas);

            fieldBaseGameObject = new GameObject("HexField");
            _cells = new HexCell[x * y];
            _width = x;
            _height = y;

            SpawnField();
        }

        [EditorButton]
        private void LoadMap(string fileName)
        {
            if (File.Exists(Application.persistentDataPath
                            + $"/{fileName}.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file =
                    File.Open(Application.persistentDataPath
                              + $"/{fileName}.dat", FileMode.Open);
                GridToSave data = (GridToSave)bf.Deserialize(file);
                file.Close();

                if (fieldBaseGameObject != null)
                {
                    DestroyImmediate(fieldBaseGameObject);
                }

                if (gridCanvasInstance == null)
                {
                    DestroyImmediate(gridCanvasInstance);
                }

                gridCanvasInstance = Instantiate(gridCanvas);

                fieldBaseGameObject = new GameObject("HexField");
                _height = data.height;
                _width = data.width;
                _cells = new HexCell[_width * _height];


                foreach (var cell in data.cells)
                {
                    if (cell == null)
                        continue;
                    CreateCell(cell.HexCoordinates.X, cell.HexCoordinates.Z, cell.index);
                }

                Debug.Log("Game data loaded!");
            }


            else
                Debug.LogError("There is no save data!");
        }

        [EditorButton]
        void SaveGrid(string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath
                                          + $"/{fileName}.dat");
            GridToSave data = new GridToSave();
            var tmp = new List<SerializibleHexCell>();
            _cells.ToList().ForEach(cell => { tmp.Add(cell == null ? null : cell.ToSerializibleHexCell()); });

            data.cells = tmp.ToArray();
            data.width = _width;
            data.height = _height;

            bf.Serialize(file, data);
            file.Close();
            Debug.Log("Game data saved!");
        }


        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new BinaryFormatter();
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

            if (Physics.Raycast(inputRay, out var hit))
            {
                var coord = HexCoordinates.FromPosition(hit.transform.position);
                _cells.First(x => x.coordinates.Equals(coord)).gameObject.GetComponent<MeshRenderer>().material.color =
                    Color.green;
            }
        }

        private void CreateCell(int x, int z, int i, bool isHexCoord = false)
        {
            Vector3 position;
            var cellGO = Object.Instantiate(hexPrefab);
            HexCell cell = _cells[i] = cellGO.AddComponent<HexCell>();
            if (isHexCoord)
            {
                HexCoordinates coordinates = new HexCoordinates(x, z);
                position = HexCoordinates.ToPosition(coordinates);
                (x, z) = HexCoordinates.ToOffsetCoordinates(coordinates);
            }
            else
            {
                position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
                position.y = 0f;
                position.z = z * (HexMetrics.outerRadius * 1.5f);
                cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            }


            cell.transform.SetParent(fieldBaseGameObject.transform, false);
            cell.transform.localPosition = position;

            cell.index = i;

            if (x > 0)
            {
                cell.SetNeighbor(HexDirection.W, _cells[i - 1]);
            }

            if (z > 0)
            {
                if ((z & 1) == 0)
                {
                    cell.SetNeighbor(HexDirection.SE, _cells[i - _width]);
                    if (x > 0)
                    {
                        cell.SetNeighbor(HexDirection.SW, _cells[i - _width - 1]);
                    }
                }
                else
                {
                    cell.SetNeighbor(HexDirection.SW, _cells[i - _width]);


                    if (x < _width - 1)
                    {
                        cell.SetNeighbor(HexDirection.SE, _cells[i - _width + 1]);
                    }
                }
            }
#if UNITY_EDITOR
            TMP_Text label = Object.Instantiate(labelPrefab, gridCanvasInstance.transform, false);
            label.rectTransform.anchoredPosition =
                new Vector2(position.x, position.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();
#endif
        }


        private void SpawnField()
        {
            for (int z = 0, i = 0; z < _height; z++)
            {
                for (int x = 0; x < _width; x++)
                {
                    CreateCell(x, z, i++);
                }
            }
        }
    }
}