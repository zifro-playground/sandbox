using UnityEngine;
using UnityEngine.EventSystems;
using Zifro.Sandbox.Entities;
using Zifro.Sandbox.UI.WorldEdit;

namespace Zifro.Sandbox.UI
{
	public class AgentDragAndDrop : MonoBehaviour,
		IBeginDragHandler,
		IDragHandler,
		IEndDragHandler
	{
		public WorldEditToolsList toolsList;
		public AgentMenuList menuList;
		public string cancelInput = "Cancel";
		public Material dragMaterial;

		bool isDragging;
		WorldEditTool lastTool;
		GameObject preview;
		Camera gameCamera;
		GridWorld gridWorld;

		void OnEnable()
		{
			gameCamera = Camera.main;
			Debug.Assert(gameCamera, $"Main camera not found in {name}.", this);
			gridWorld = GridWorld.main;
			Debug.Assert(gridWorld, $"Main grid world not found in {name}.", this);
		}

		void Awake()
		{
			Debug.Assert(toolsList, $"{nameof(toolsList)} not defined in {name}.", this);
			Debug.Assert(menuList, $"{nameof(menuList)} not defined in {name}.", this);
		}

		void Update()
		{
			if (!isDragging)
			{
				return;
			}

			if (Input.GetButtonDown(cancelInput))
			{
				// Cancel drag
				DragEndOrCancel();
				return;
			}

			Vector3 point = gameCamera.ScreenToWorldPoint(Input.mousePosition);

			if (gridWorld.TryRaycastBlocks(point, gameCamera.transform.forward, gameCamera.farClipPlane,
				out GridRaycastHit hit))
			{
				preview.transform.position = gridWorld.VoxelToWorld(hit.voxelIndex + hit.voxelNormal) - new Vector3(0, 0.5f, 0);
				preview.gameObject.SetActive(true);
			}
			else
			{
				preview.gameObject.SetActive(false);
			}
		}

		void OnDisable()
		{
			if (preview)
			{
				Destroy(preview);
				preview = null;
			}
		}

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			if (!menuList.current || !menuList.current.modelPrefab)
			{
				// Stop drag events
				eventData.pointerDrag = null;
				return;
			}

			isDragging = true;
			lastTool = toolsList.currentTool;

			if (lastTool)
			{
				toolsList.DeselectToolWithoutUIUpdate();
			}

			preview = Instantiate(menuList.current.modelPrefab);

			if (dragMaterial)
			{
				foreach (Renderer child in preview.GetComponentsInChildren<Renderer>())
				{
					child.sharedMaterial = dragMaterial;
				}
			}
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			if (!isDragging)
			{
				// Stop drag events
				eventData.pointerDrag = null;
			}
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			if (!isDragging)
			{
				return;
			}

			DragEndOrCancel();

			// Release the item, successful end drag

		}

		void DragEndOrCancel()
		{
			isDragging = false;

			if (lastTool)
			{
				toolsList.SelectTool(lastTool);
			}

			if (preview)
			{
				Destroy(preview);
				preview = null;
			}
		}
	}
}
