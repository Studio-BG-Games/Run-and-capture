using System;
using UnityEngine;

namespace HexFiled
{
	[System.Serializable]
	public struct HexCoordinates : IComparable<HexCoordinates> {

		[SerializeField]
		private int x, z;

		public int X => x;

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			var coord = (HexCoordinates)obj;
			
			return coord.X == X && coord.Y == Y && coord.Z == Z;
		}

		public bool Equals(HexCoordinates other)
		{
			return x == other.x && z == other.z;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (x * 397) ^ z;
			}
		}

		public int Z => z;

		public int Y => -X - Z;

		public HexCoordinates (int x, int z) {
			this.x = x;
			this.z = z;
		}

		public static HexCoordinates FromOffsetCoordinates (int x, int z) {
			return new HexCoordinates(x - z / 2, z);
		}

		public static (int x, int z) ToOffsetCoordinates(HexCoordinates coordinates)
		{
			return (coordinates.X + coordinates.Z / 2, coordinates.Z);
		}
		public static HexCoordinates FromPosition (Vector3 position) {
			float x = position.x / (HexMetrics.innerRadius * 2f);
			float y = -x;

			float offset = position.z / (HexMetrics.outerRadius * 3f);
			x -= offset;
			y -= offset;

			int iX = Mathf.RoundToInt(x);
			int iY = Mathf.RoundToInt(y);
			int iZ = Mathf.RoundToInt(-x -y);

			if (iX + iY + iZ != 0) {
				float dX = Mathf.Abs(x - iX);
				float dY = Mathf.Abs(y - iY);
				float dZ = Mathf.Abs(-x -y - iZ);

				if (dX > dY && dX > dZ) {
					iX = -iY - iZ;
				}
				else if (dZ > dY) {
					iZ = -iX - iY;
				}
			}

			return new HexCoordinates(iX, iZ);
		}

		public static Vector3 ToPosition(HexCoordinates position) {
			
			/*
			position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.outerRadius * 1.5f);
                */
			var hexPos = ToOffsetCoordinates(position);
			Vector3 pos;
			pos.x = (hexPos.x + hexPos.z * 0.5f - hexPos.z / 2) * (HexMetrics.innerRadius * 2f);
			pos.y = 0f;
			pos.z = hexPos.z * (HexMetrics.outerRadius * 1.5f);
			
			return pos;
		}
		public override string ToString () {
			return "(" +
			       X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
		}

		

		public string ToStringOnSeparateLines () {
			return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
		}

		public int CompareTo(HexCoordinates other)
		{
			var xComparison = x.CompareTo(other.X);
			if (xComparison != 0) return xComparison;
			return z.CompareTo(other.Z);
		}
	}
}