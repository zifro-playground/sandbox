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
		public GridWorld gridWorld;
		public GridWorldSelector selectionHighlight;
		public float maxRaycastDistance = 50;

		bool pointerOver;

		[SerializeField, HideInInspector]
		new Camera camera;

		void Awake()
		{
			camera = Camera.main;
			Debug.Assert(camera, "Main camera not found.", this);
			Debug.Assert(gridWorld, $"{nameof(gridWorld)} not assigned.", this);
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

			Vector3 from = camera.ScreenToWorldPoint(Input.mousePosition);

			Vector3 forward = camera.transform.forward;
			Debug.DrawRay(from, forward * maxRaycastDistance, Color.red);

			if (!gridWorld.TryRaycastBlocks(from, forward, maxRaycastDistance, out GridRaycastHit hit))
			{
				selectionHighlight.DeselectAll();
				return;
			}

			selectionHighlight.SelectVoxel(hit.voxelIndex, hit.normal);
			bool modifier = IDESpecialCommands.AnyKey(KeyCode.LeftControl, KeyCode.RightControl);
			bool lmb = Input.GetMouseButtonDown(0);
			bool rmb = Input.GetMouseButtonDown(1);

			bool addButton = lmb && !modifier;
			bool removeButton = rmb || (lmb && modifier);

			if (addButton)
			{
				gridWorld.SetBlock(hit.voxelIndex + hit.voxelNormal);
			}
			else if (removeButton)
			{
				gridWorld.RemoveBlock(hit.voxelIndex);
			}
		}
	}
}
