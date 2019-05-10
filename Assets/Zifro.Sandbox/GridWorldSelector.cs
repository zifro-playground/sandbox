using UnityEngine;

namespace Zifro.Sandbox
{
	public class GridWorldSelector : MonoBehaviour
	{
		public GridWorld gridWorld;
		public Vector3 outerPadding = new Vector3(0.1f, 0.1f, 0.1f);

		void Awake()
		{
			Debug.Assert(gridWorld, $"{nameof(gridWorld)} not assigned.", this);
		}

		public void SelectVoxel(Vector3Int voxel)
		{
			gameObject.SetActive(true);
			Transform self = transform;
			self.position = gridWorld.VoxelToWorld(voxel);
			self.localScale = Vector3.one + outerPadding * 2;
		}

		public void SelectVoxel(Vector3Int voxel, Vector3 normal)
		{
			gameObject.SetActive(true);

			Vector3 normalized = normal.normalized;
			Transform self = transform;
			self.position = gridWorld.VoxelToWorld(voxel)
				+ normalized * 0.5f;

			self.localScale = new Vector3(
				1 - Mathf.Abs(normalized.x) + outerPadding.x,
				1 - Mathf.Abs(normalized.y) + outerPadding.y,
				1 - Mathf.Abs(normalized.z) + outerPadding.z
			);
		}

		public void DeselectAll()
		{
			gameObject.SetActive(false);
		}
	}
}
