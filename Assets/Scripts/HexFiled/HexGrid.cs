using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HexFiled
{
    [Serializable]
    public class HexGrid 
    {
        
        private HexCell[] _cells;
        private Canvas _gridCanvas;
        private GameObject _baseGameObject;
        private FieldData _fieldData;

        public static float HexDistance => _hexDistance;
        public event Action<HexCell> OnHexPainted;
        public event Action OnGridLoaded;
        
        private static Dictionary<UnitColor, CellColor> _colors;
        private static float _hexDistance;
        public int HexCaptureCost => _fieldData.hexCaptureManaCost;

        public int HexHardCaptureCost => _fieldData.hexHardCaptureManaCost;
        public float HardCaptureTime => _fieldData.hexHardCaptureTime;
        

        public static Dictionary<UnitColor, CellColor> Colors => _colors;
        

        public HexGrid(FieldData fieldData)
        {
            _fieldData = fieldData;
            _baseGameObject = new GameObject("HexGrid");
            _colors = fieldData.colors;
            

            _gridCanvas = Object.Instantiate(fieldData.CoordinatesCanvas, _baseGameObject.transform)
                .GetComponent<Canvas>();
            HexManager.CellByColor = new Dictionary<UnitColor, List<HexCell>>();
            _cells = new HexCell[_fieldData.height * _fieldData.width];
        }

       

        public HexCell GetCellFromCoord(HexCoordinates coordinates)
        {
            return _cells.First(cell => cell.coordinates.Equals(coordinates));
        }


        private void CreateCell(int x, int z, int i)
        {
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.outerRadius * 1.5f);
            var cellGO = Object.Instantiate(_fieldData.cellPrefab);
            HexCell cell = _cells[i] = cellGO.GetComponent<HexCell>();
            cell.PaintHex(UnitColor.Grey, true);
            cell.transform.SetParent(_baseGameObject.transform, false);
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.OnHexPainted += OnHexPainted;

            if (x > 0)
            {
                cell.SetNeighbor(HexDirection.W, _cells[i - 1]);
            }

            if (z > 0)
            {
                if ((z & 1) == 0)
                {
                    cell.SetNeighbor(HexDirection.SE, _cells[i - _fieldData.width]);
                    if (x > 0)
                    {
                        cell.SetNeighbor(HexDirection.SW, _cells[i - _fieldData.width - 1]);
                    }
                }
                else
                {
                    cell.SetNeighbor(HexDirection.SW, _cells[i - _fieldData.width]);
                    if (_hexDistance == 0f)
                    {
                        _hexDistance = Vector3.Distance(cell.transform.position,
                            cell.GetNeighbor(HexDirection.SW).transform.position);
                    }

                    if (x < _fieldData.width - 1)
                    {
                        cell.SetNeighbor(HexDirection.SE, _cells[i - _fieldData.width + 1]);
                    }
                }
            }


#if UNITY_EDITOR
            TMP_Text label = Object.Instantiate(_fieldData.cellLabelPrefab, _gridCanvas.transform, false);
            label.rectTransform.anchoredPosition =
                new Vector2(position.x, position.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();
#endif
        }


        public GameObject SpawnField()
        {
            
            for (int z = 0, i = 0; z < _fieldData.height; z++)
            {
                for (int x = 0; x < _fieldData.width; x++)
                {
                    CreateCell(x, z, i++);
                }
            }

            OnGridLoaded?.Invoke();
            return _baseGameObject;
        }
    }
}