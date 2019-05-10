using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zifro.Sandbox.Entities
{
	public struct GridRaycastHit : IEquatable<GridRaycastHit>
	{
		public Vector3 point { get; set; }
		public Vector3 normal => world.VoxelNormalToWorld(voxelNormal);
		public Vector3 voxelPosition => world.VoxelToWorld(voxelIndex);
		public Vector3Int voxelIndex { get; set; }
		public Vector3Int voxelNormal { get; set; }
		public float distance { get; set; }
		public GridWorld world { get; set; }

		public override bool Equals(object obj)
		{
			return obj is GridRaycastHit hit && Equals(hit);
		}

		public bool Equals(GridRaycastHit other)
		{
			return point.Equals(other.point) &&
				   normal.Equals(other.normal) &&
				   voxelPosition.Equals(other.voxelPosition) &&
				   voxelIndex.Equals(other.voxelIndex) &&
				   Math.Abs(distance - other.distance) < 0.0001f;
		}

		public override int GetHashCode()
		{
			int hashCode = 1878262334;
			hashCode = hashCode * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(point);
			hashCode = hashCode * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(normal);
			hashCode = hashCode * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(voxelPosition);
			hashCode = hashCode * -1521134295 + EqualityComparer<Vector3Int>.Default.GetHashCode(voxelIndex);
			hashCode = hashCode * -1521134295 + distance.GetHashCode();
			return hashCode;
		}

		public static bool operator ==(GridRaycastHit hit1, GridRaycastHit hit2)
		{
			return hit1.Equals(hit2);
		}

		public static bool operator !=(GridRaycastHit hit1, GridRaycastHit hit2)
		{
			return !(hit1 == hit2);
		}
	}
}
