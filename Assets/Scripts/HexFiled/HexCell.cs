using System;
using UnityEngine;

namespace HexFiled
{
	public class HexCell : MonoBehaviour
	{

		public HexCoordinates coordinates;
		public Color color;
		public Action<HexCell> OnHexPainted;

		[SerializeField] private HexCell[] neighbors;
		private static readonly int Player = Shader.PropertyToID("player");

		public HexCell GetNeighbor(HexDirection direction)
		{
			return neighbors[(int)direction];
		}

		public void SetNeighbor(HexDirection direction, HexCell cell)
		{
			neighbors[(int)direction] = cell;
			cell.neighbors[(int)direction.Opposite()] = this;
		}

		public void PaintHex(Texture texture)
		{
			gameObject.GetComponent<MeshRenderer>().material.mainTexture = texture;
			OnHexPainted?.Invoke(this);
		}
	}
}
