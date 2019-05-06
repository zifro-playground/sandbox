using System;
using UnityEngine;

namespace Zifro.Sandbox.Entities
{
	[Serializable]
	public struct FractionVector3 : IEquatable<FractionVector3>
	{
		public const int SCALE = 20;
		public const float SCALE_INVERSE = 1/20f;

		public FractionVector3(float x, float y, float z)
		{
			fractionX = Mathf.RoundToInt(x * SCALE);
			fractionY = Mathf.RoundToInt(y * SCALE);
			fractionZ = Mathf.RoundToInt(z * SCALE);
		}

		public FractionVector3(int fractionX, int fractionY, int fractionZ)
		{
			this.fractionX = fractionX;
			this.fractionY = fractionY;
			this.fractionZ = fractionZ;
		}

		[field: SerializeField]
		public int fractionX { get; set; }
		[field: SerializeField]
		public int fractionY { get; set; }
		[field: SerializeField]
		public int fractionZ { get; set; }

		public float x
		{
			get => fractionX / (float)SCALE;
			set => fractionX = Mathf.RoundToInt(value * SCALE);
		}
		public float y
		{
			get => fractionY / (float)SCALE;
			set => fractionY = Mathf.RoundToInt(value * SCALE);
		}
		public float z
		{
			get => fractionZ / (float)SCALE;
			set => fractionZ = Mathf.RoundToInt(value * SCALE);
		}

		public static implicit operator Vector3(FractionVector3 fraction)
		{
			return new Vector3(fraction.x, fraction.y, fraction.z);
		}

		public static explicit operator FractionVector3(Vector3 vector)
		{
			return new FractionVector3(vector.x, vector.y, vector.z);
		}

		public static explicit operator Vector3Int(FractionVector3 fraction)
		{
			return new Vector3Int(fraction.fractionX / SCALE, fraction.fractionY / SCALE, fraction.fractionZ / SCALE);
		}

		public static implicit operator FractionVector3(Vector3Int vector)
		{
			return new FractionVector3(vector.x * SCALE, vector.y * SCALE, vector.z * SCALE);
		}

		public static FractionVector3 operator +(FractionVector3 a, FractionVector3 b)
		{
			return new FractionVector3(a.fractionX + b.fractionX, a.fractionY + b.fractionY, a.fractionZ + b.fractionZ);
		}

		public static FractionVector3 operator -(FractionVector3 a, FractionVector3 b)
		{
			return new FractionVector3(a.fractionX - b.fractionX, a.fractionY - b.fractionY, a.fractionZ - b.fractionZ);
		}

		public static FractionVector3 operator -(FractionVector3 a)
		{
			return new FractionVector3(-a.fractionX, -a.fractionY, -a.fractionZ);
		}

		public static FractionVector3 operator *(FractionVector3 a, int scale)
		{
			return new FractionVector3(a.fractionX * scale, a.fractionY * scale, a.fractionZ * scale);
		}

		public static FractionVector3 operator *(FractionVector3 a, float scale)
		{
			return new FractionVector3(a.x * scale, a.y * scale, a.z * scale);
		}

		public static FractionVector3 operator /(FractionVector3 a, int scale)
		{
			return new FractionVector3(a.fractionX / scale, a.fractionY / scale, a.fractionZ / scale);
		}

		public static FractionVector3 operator /(FractionVector3 a, float scale)
		{
			return new FractionVector3(a.x / scale, a.y / scale, a.z / scale);
		}

		public static bool operator ==(FractionVector3 vector1, FractionVector3 vector2)
		{
			return vector1.Equals(vector2);
		}

		public static bool operator !=(FractionVector3 vector1, FractionVector3 vector2)
		{
			return !(vector1 == vector2);
		}

		public override bool Equals(object obj)
		{
			return obj is FractionVector3 fraction && Equals(fraction);
		}

		public bool Equals(FractionVector3 other)
		{
			return fractionX == other.fractionX &&
			       fractionY == other.fractionY &&
			       fractionZ == other.fractionZ;
		}

		public override int GetHashCode()
		{
			int hashCode = -773955598;
			hashCode = hashCode * -1521134295 + fractionX.GetHashCode();
			hashCode = hashCode * -1521134295 + fractionY.GetHashCode();
			hashCode = hashCode * -1521134295 + fractionZ.GetHashCode();
			return hashCode;
		}
	}
}
