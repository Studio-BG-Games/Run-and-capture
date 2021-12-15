using System;
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
        private Color _defaultColor = Color.white;
        private Color _touchedColor = Color.magenta;
        private GameObject _cellPrefab;
        private TMP_Text _cellLabelPrefab;
        private Camera _camera;
        private HexCell[] _cells;
        private Canvas _gridCanvas;
        private GameObject _baseGameObject;
        public Action<HexCell> OnHexPainted;
        public Action OnGridLoaded;

        public HexGrid(FieldData fieldData)
        {
            _width = fieldData.width;
            _height = fieldData.height;
            _defaultColor = fieldData.defaultColor;
            _touchedColor = fieldData.touchedColor;
            _cellPrefab = fieldData.cellPrefab;
            _cellLabelPrefab = fieldData.cellLabelPrefab;
            _camera = Camera.main;
            _baseGameObject = new GameObject("HexGrid");

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

        private void PaintHex(Color color, HexCoordinates coordinates)
        {
            int index = coordinates.X + coordinates.Z * _width + coordinates.Z / 2;
            HexCell cell = _cells[index];
            
            cell.color = color;
            cell.gameObject.GetComponent<MeshRenderer>().material.color = color;
            OnHexPainted.Invoke(_cells[index]);
        }

        void CreateCell(int x, int z, int i)
        {
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.outerRadius * 1.5f);
            var cellGO = Object.Instantiate(_cellPrefab);
            HexCell cell = _cells[i] = cellGO.GetComponent<HexCell>();
            cell.transform.SetParent(_baseGameObject.transform, false);
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.color = _defaultColor;
            cell.OnHexPainted += OnHexPainted;
            
            if (x > 0) {
                cell.SetNeighbor(HexDirection.W, _cells[i - 1]);
            }
            
            if (z > 0) {
                if ((z & 1) == 0) {
                    cell.SetNeighbor(HexDirection.SE, _cells[i - _width]);
                    if (x > 0) {
                        cell.SetNeighbor(HexDirection.SW, _cells[i - _width - 1]);
                    }
                }
                else {
                    cell.SetNeighbor(HexDirection.SW, _cells[i - _width]);
                    if (x < _width - 1) {
                        cell.SetNeighbor(HexDirection.SE, _cells[i - _width + 1]);
                    }
                }
            }

            TMP_Text label = Object.Instantiate(_cellLabelPrefab, _gridCanvas.transform, false);
            label.rectTransform.anchoredPosition =
                new Vector2(position.x, position.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();
        }

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

            // _hexMesh.Triangulate(_cells);
            OnGridLoaded.Invoke();
        }
        
    }
}