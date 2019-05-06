using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridWorldTester : MonoBehaviour
{
	public GridWorld world;

	void OnDrawGizmos()
	{
		if (!world)
		{
			return;
		}

		Transform self = transform;
		Vector3 position = self.position;
		Vector3 direction = self.forward;
		float distance = self.localScale.magnitude;

		if (world.TryRaycastBlocks(position, direction, distance,
			out RaycastHit hit))
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(position, hit.point);
			Gizmos.DrawSphere(hit.point, 0.1f);
		}
		else
		{
			Gizmos.color = Color.green;
			Gizmos.DrawRay(position, direction * distance);
			Gizmos.DrawSphere(position + direction * distance, 0.1f);
		}
	}
}
