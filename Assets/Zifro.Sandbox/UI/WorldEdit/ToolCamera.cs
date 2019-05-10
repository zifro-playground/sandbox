using UnityEngine;
using UnityEngine.EventSystems;

namespace Zifro.Sandbox.UI.WorldEdit
{
	public class ToolCamera : WorldEditTool,
		IToolScreenEnter,
		IToolScreenExit
	{

		public float minZoomIn = 2;
		public float maxZoomOut = 10;

		[SerializeField, HideInInspector]
		Camera gameCamera;

		bool pointerOver;

		void OnValidate()
		{
			minZoomIn = Mathf.Clamp(minZoomIn, 0, maxZoomOut);
			maxZoomOut = Mathf.Max(maxZoomOut, minZoomIn);
		}

		void Awake()
		{
			gameCamera= Camera.main;
			Debug.Assert(gameCamera, "Main camera not found.", this);
		}

		void Update()
		{
			float zoom = gameCamera.orthographicSize + Input.mouseScrollDelta.y;
			gameCamera.orthographicSize = Mathf.Clamp(zoom, minZoomIn, maxZoomOut);
		}

		public override void OnToolSelected()
		{
			enabled = pointerOver && isSelected;
		}

		public override void OnToolDeselected()
		{
			enabled = false;
		}

		void IToolScreenEnter.OnScreenEnter(PointerEventData eventData)
		{
			pointerOver = true;
			enabled = pointerOver && isSelected;
		}

		void IToolScreenExit.OnScreenExit(PointerEventData eventData)
		{
			pointerOver = false;
			enabled = false;
		}
	}
}
