using System;
using UnityEngine;
using UnityEngine.UI;

namespace Zifro.Sandbox.UI.WorldEdit
{
	public abstract class WorldEditTool : MonoBehaviour
	{
		public Button button;
		public KeyCode hotKey;

		[NonSerialized]
		public bool isSelected;

		[NonSerialized]
		public bool isMouseOverGame;

		[NonSerialized]
		public GridWorld world;

		[NonSerialized]
		public Camera gameCamera;

		public abstract void OnToolSelectedChange(WorldEditTool lastTool);
		public abstract void OnMouseOverChange();
	}
}
