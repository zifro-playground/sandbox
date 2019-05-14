using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTester : MonoBehaviour
{
	MeshFilter filter;

	void OnValidate()
	{
		filter = GetComponent<MeshFilter>();
		if (filter)
		{
			if (!filter.sharedMesh)
			{
				filter.sharedMesh = new Mesh();
			}

			RedrawMesh(filter.sharedMesh);
		}
	}

	static void RedrawMesh(Mesh mesh)
	{
		mesh.Clear();
		mesh.vertices = new[] {new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 0, 0)};
		mesh.uv = new[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0)};
		mesh.triangles = new[] {0, 1, 2, 0, 2, 3};
		mesh.RecalculateNormals();
		mesh.colors32 = new[] {
			new Color32(255, 255, 255, 255), new Color32(0, 0, 0, 255), new Color32(255, 255, 255, 255),
			new Color32(255, 255, 255, 255)
		};
	}
}
