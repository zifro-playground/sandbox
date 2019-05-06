using System.Linq;
using UnityEditor;
using UnityEngine;

public class GridWorld : MonoBehaviour
{
	public GameObject blockPrefab;

	[TextArea]
	public string[] initializationLayers;

	bool[,,] grid;

	void Awake()
	{
		RegenerateGrid();
	}

//#if UNITY_EDITOR
//	public bool regenerate;
//	void OnValidate()
//	{
//		if (blockPrefab && regenerate)
//		{
//			regenerate = false;
//			RegenerateGrid();
//		}
//	}
//#endif

	private void RegenerateGrid()
	{
		// Remove old
		foreach (Transform child in transform)
		{

//#if UNITY_EDITOR
//			if (!EditorApplication.isPlaying)
//			{
//				DestroyImmediate(child.gameObject);
//				continue;
//			}
//#endif
			Destroy(child.gameObject);
		}

		// Add new
		string[][] splitByLine = initializationLayers.Select(o => o.Split('\n')).ToArray();

		int height = splitByLine.Length;
		int width = splitByLine.Max(o => o.Length);
		int length = splitByLine.Max(o => o.Max(n => n.Length));

		grid = new bool[width, height, length];
		Transform parent = transform;

		for (int y = 0; y < splitByLine.Length; y++)
		{
			string[] layer = splitByLine[y];
			for (int x = 0; x < layer.Length; x++)
			{
				string line = layer[x];
				for (int z = 0; z < line.Length; z++)
				{
					if (!char.IsLetterOrDigit(line[z]))
					{
						continue;
					}

					grid[x, y, z] = true;
					Instantiate(blockPrefab,
						parent.TransformPoint(x + 0.5f, y + 0.5f, z + 0.5f),
						Quaternion.identity,
						parent);
				}
			}
		}
	}

	public bool IsPointInBlock(float x, float y, float z)
	{
		Vector3 localPoint = transform.InverseTransformPoint(x, y, z);
		return IsLocalPointInBlock(localPoint);
	}

	public bool IsPointInBlock(Vector3 point)
	{
		Vector3 localPoint = transform.InverseTransformPoint(point);
		return IsLocalPointInBlock(localPoint);
	}

	private bool IsLocalPointInBlock(Vector3 localPoint)
	{
		if (grid == null)
		{
			return false;
		}

		if (localPoint.x >= 0 && localPoint.x < grid.GetLength(0) &&
			localPoint.y >= 0 && localPoint.y < grid.GetLength(1) &&
			localPoint.z >= 0 && localPoint.z < grid.GetLength(2))
		{
			return grid[(int)localPoint.x, (int)localPoint.y, (int)localPoint.z];
		}

		return false;
	}
}
