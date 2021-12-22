using System;
using System.Collections.Generic;
using Runtime.Controller;
using Runtime.Data;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HexFiled
{
    public class HexGrid : IInitialization
    {
        private int _width;
        private int _height;
        private GameObject _cellPrefab;
        private TMP_Text _cellLabelPrefab;
        private HexCell[] _cells;
        private Canvas _gridCanvas;
        private GameObject _baseGameObject;
        private Dictionary<UnitColor, CellColor> _colors;
        private float _hexDistance;

        public float HexDistance => _hexDistance;
        public Action<HexCell> OnHexPainted;
        public Action OnGridLoaded;

        public HexGrid(FieldData fieldData)
        {
            _width = fieldData.width;
            _height = fieldData.height;
            _cellPrefab = fieldData.cellPrefab;
            _cellLabelPrefab = fieldData.cellLabelPrefab;
            _baseGameObject = new GameObject("HexGrid");
            _colors = new Dictionary<UnitColor, CellColor>(fieldData.colors.Count);
            foreach (var color in fieldData.colors)
            {
                _colors.Add(color.UnitColor, color);
            }

            _gridCanvas = Object.Instantiate(fieldData.CoordinatesCanvas, _baseGameObject.transform)
                .GetComponent<Canvas>();
        }

        public HexCell GetCellFromCoord(HexCoordinates coordinates)
        {
            var i = 0;
            while (!_cells[i++].coordinates.Equals(coordinates))
            {
            }

            return _cells[i - 1];
        }


        void CreateCell(int x, int z, int i)
        {
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.outerRadius * 1.5f);
            var cellGO = Object.Instantiate(_cellPrefab);
            HexCell cell = _cells[i] = cellGO.GetComponent<HexCell>();
            cell.SetDictionary(_colors);
            cell.PaintHex(UnitColor.GREY);
            cell.transform.SetParent(_baseGameObject.transform, false);
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.onHexPainted += OnHexPainted;

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
                    if (_hexDistance == 0f)
                    {
                        _hexDistance = Vector3.Distance(cell.transform.position,
                            cell.GetNeighbor(HexDirection.SW).transform.position);
                    }

                    if (x < _width - 1)
                    {
                        cell.SetNeighbor(HexDirection.SE, _cells[i - _width + 1]);
                    }
                }
            }
        }
// #if UNITY_EDITOR
//             TMP_Text label = Object.Instantiate(_cellLabelPrefab, _gridCanvas.transform, false);
//             label.rectTransform.anchoredPosition =
//                 new Vector2(position.x, position.z);
//             label.text = cell.coordinates.ToStringOnSeparateLines();
// #endif


        public void Init()
        {
            _cells = new HexCell[_height * _width];

            for (int z = 0, i = 0; z < _height; z++)
            {
                for (int x = 0; x < _width; x++)
                {
                    CreateCell(x, z, i++);
                }
            }

            OnGridLoaded.Invoke();
        }
    }
}