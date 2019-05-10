using UnityEngine;
using UnityEngine.UI;

namespace Zifro.Sandbox.UI.WorldEdit
{
	public abstract class WorldEditTool : MonoBehaviour
	{
		public KeyCode hotKey;

		[HideInInspector]
		public bool isSelected;

		[HideInInspector]
		public Button button;

		[HideInInspector]
		public GridWorld world;

		[HideInInspector]
		public Camera gameCamera;

		public abstract void OnToolSelected();
		public abstract void OnToolDeselected();
	}
}
