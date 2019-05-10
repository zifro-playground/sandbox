using UnityEngine;
using UnityEngine.EventSystems;

namespace Zifro.Sandbox.UI.WorldEdit
{
	public class ToolEditBlocks : WorldEditTool,
		IToolScreenEnter,
		IToolScreenExit
	{
		public GridWorld gridWorld;
		public GameObject selectionHighlight;
		public float maxRaycastDistance = 50;

		public bool pointerOver;

		[SerializeField, HideInInspector]
		new Camera camera;

		void Awake()
		{
			camera = Camera.main;
			Debug.Assert(camera, "Main camera not found.", this);
			Debug.Assert(gridWorld, "GridWorld not assigned.", this);
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
			selectionHighlight.SetActive(false);
		}

		public override void OnToolSelected()
		{
			enabled = true;
		}

		public override void OnToolDeselected()
		{
			enabled = false;
			selectionHighlight.SetActive(false);
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

			if (gridWorld.TryRaycastBlocks(from, forward, maxRaycastDistance, out RaycastHit hit))
			{
				selectionHighlight.SetActive(true);
				selectionHighlight.transform.position = new Vector3(
					Mathf.Floor(hit.point.x) + 0.5f,
					Mathf.Floor(hit.point.y) + 0.5f,
					Mathf.Floor(hit.point.z) + 0.5f
				);
			}
			else
			{
				selectionHighlight.SetActive(false);
			}
		}
	}
}
