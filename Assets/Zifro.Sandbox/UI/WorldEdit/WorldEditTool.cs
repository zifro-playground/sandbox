using System;
using UnityEngine;
using UnityEngine.UI;

namespace Zifro.Sandbox.UI.WorldEdit
{
	public abstract class WorldEditTool : MenuItem
	{
		public KeyCode hotKey;

		[NonSerialized]
		public bool isMouseOverGame;

		[NonSerialized]
		public GridWorld world;

		[NonSerialized]
		public Camera gameCamera;

		protected new void Start()
		{
			base.Start();
			gameCamera = Camera.main;
			Debug.Assert(gameCamera, "Main camera not found.", this);
		}

		public abstract void OnMouseOverChange();
	}
}
