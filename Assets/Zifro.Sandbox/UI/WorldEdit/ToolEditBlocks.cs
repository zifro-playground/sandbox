using PM;
using UnityEngine;
using UnityEngine.EventSystems;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox.UI.WorldEdit
{
	public class ToolEditBlocks : WorldEditTool,
		IToolScreenEnter,
		IToolScreenExit
	{
		public GridWorldHighlight selectionHighlight;

		bool pointerOver;

		void Awake()
		{
			gameCamera = Camera.main;
			Debug.Assert(gameCamera, "Main camera not found.", this);
			Debug.Assert(selectionHighlight, $"{nameof(selectionHighlight)} not assigned.", this);
		}

		void Start()
		{
			enabled = false;
		}

		void IToolScreenEnter.OnScreenEnter(PointerEventData eventData)
		{
			pointerOver = true;
		}

		void IToolScreenExit.OnScreenExit(PointerEventData eventData)
		{
			pointerOver = false;
			selectionHighlight.DeselectAll();
		}

		public override void OnToolSelected()
		{
			enabled = true;
		}

		public override void OnToolDeselected()
		{
			enabled = false;
			selectionHighlight.DeselectAll();
		}

		void Update()
		{
			if (!pointerOver)
			{
				return;
			}

			Vector3 from = gameCamera.ScreenToWorldPoint(Input.mousePosition);

			Vector3 forward = gameCamera.transform.forward;
			Debug.DrawRay(from, forward * gameCamera.farClipPlane, Color.red);

			if (!world.TryRaycastBlocks(from, forward, gameCamera.farClipPlane, out GridRaycastHit hit))
			{
				selectionHighlight.DeselectAll();
				return;
			}

			bool modifier = IDESpecialCommands.AnyKey(KeyCode.LeftControl, KeyCode.RightControl);

			if (modifier)
			{
				selectionHighlight.SelectVoxel(hit.voxelIndex);
			}
			else
			{
				selectionHighlight.SelectVoxel(hit.voxelIndex, hit.voxelNormal);
			}

			bool lmb = Input.GetMouseButtonDown(0);
			bool rmb = Input.GetMouseButtonDown(1);

			bool addButton = lmb && !modifier;
			bool removeButton = rmb || (lmb && modifier);

			if (addButton)
			{
				world.SetBlock(hit.voxelIndex + hit.voxelNormal);
			}
			else if (removeButton)
			{
				world.RemoveBlock(hit.voxelIndex);
			}
		}
	}
}
