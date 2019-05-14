using UnityEngine;

namespace Zifro.Sandbox
{
	public class GridWorldHighlight : MonoBehaviour
	{
		public GridWorld gridWorld;
		public Vector3 outerPadding = new Vector3(0.1f, 0.1f, 0.1f);

		Vector3Int lastVoxel;
		Vector3Int lastNormal;

		void Awake()
		{
			Debug.Assert(gridWorld, $"{nameof(gridWorld)} not assigned.", this);

			lastVoxel = gridWorld.WorldToVoxel(transform.position);
		}

		public void SelectVoxel(Vector3Int voxel)
		{
			SelectVoxel(voxel, Vector3Int.zero);
		}

		public void SelectVoxel(Vector3Int voxel, Vector3Int normal)
		{
			if (lastVoxel == voxel && lastNormal == normal)
			{
				return;
			}

			gameObject.SetActive(true);

			Transform self = transform;
			if (normal == Vector3.zero)
			{
				self.position = gridWorld.VoxelToWorld(voxel);
				self.localScale = Vector3.one + outerPadding;
			}
			else
			{
				self.position = gridWorld.VoxelToWorld(voxel)
				                + new Vector3(normal.x * 0.5f, normal.y * 0.5f, normal.z * 0.5f);

				self.localScale = new Vector3(
					1 - Mathf.Abs(normal.x) + outerPadding.x,
					1 - Mathf.Abs(normal.y) + outerPadding.y,
					1 - Mathf.Abs(normal.z) + outerPadding.z
				);
			}

			lastVoxel = voxel;
			lastNormal = normal;
		}

		public void DeselectAll()
		{
			gameObject.SetActive(false);
		}
	}
}
