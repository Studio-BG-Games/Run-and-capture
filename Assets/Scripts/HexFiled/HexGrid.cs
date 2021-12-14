using System;
using Runtime.Controller;
using Runtime.Data;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HexFiled
{
	public class HexGrid : IInitialization, IExecute {

		private int _width;
		private int _height;
		private Color _defaultColor = Color.white;
		private Color _touchedColor = Color.magenta;
		private HexCell _cellPrefab;
		private Text _cellLabelPrefab;
		private Camera _camera;
		private HexCell[] _cells;
		private Canvas _gridCanvas;
		private HexMesh _hexMesh;
		private GameObject _baseGameObject;
		public Action<HexCoordinates> OnHexPainted;

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
		
			_gridCanvas = Object.Instantiate(fieldData.CoordinatesCanvas, _baseGameObject.transform).GetComponent<Canvas>();
			_hexMesh = Object.Instantiate(fieldData.HexMeshPrefab, _baseGameObject.transform).GetComponent<HexMesh>();
		}
	

		void HandleInput () {
			Ray inputRay = _camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(inputRay, out hit)) {
				TouchCell(hit.point);
			}
		}

		public void AddColorChangeListener(Action onColorChange)
		{
			
		}
		
		void TouchCell (Vector3 position) {
			position = _baseGameObject.transform.InverseTransformPoint(position);
			HexCoordinates coordinates = HexCoordinates.FromPosition(position);
			PaintHex(_touchedColor, coordinates);
		}

		private void PaintHex(Color color, HexCoordinates coordinates)
		{
			int index = coordinates.X + coordinates.Z * _width + coordinates.Z / 2;
			HexCell cell = _cells[index];
			cell.color = _touchedColor;
			_hexMesh.Triangulate(_cells);
			OnHexPainted.Invoke(coordinates);
		}
		void CreateCell (int x, int z, int i) {
			Vector3 position;
			position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
			position.y = 0f;
			position.z = z * (HexMetrics.outerRadius * 1.5f);

			HexCell cell = _cells[i] = Object.Instantiate<HexCell>(_cellPrefab);
			cell.transform.SetParent(_baseGameObject.transform, false);
			cell.transform.localPosition = position;
			cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
			cell.color = _defaultColor;

			Text label = Object.Instantiate<Text>(_cellLabelPrefab, _gridCanvas.transform, false);
			label.rectTransform.anchoredPosition =
				new Vector2(position.x, position.z);
			label.text = cell.coordinates.ToStringOnSeparateLines();
		}

		public void Init()
		{
		
			_cells = new HexCell[_height * _width];

			for (int z = 0, i = 0; z < _height; z++) {
				for (int x = 0; x < _width; x++) {
					CreateCell(x, z, i++);
				}
			}
			_hexMesh.Triangulate(_cells);
		}

		public void Execute()
		{
			if (Input.GetMouseButton(0)) {
				HandleInput();
			}
		}
	}
}