using UnityEngine;
using UnityEngine.EventSystems;
using Zifro.Sandbox.Entities;
using Zifro.Sandbox.UI.WorldEdit;

namespace Zifro.Sandbox.UI
{
	public class AgentDragAndDrop : WorldEditTool,
		IBeginDragHandler,
		IDragHandler,
		IEndDragHandler
	{
		public WorldEditToolsList toolsList;
		public AgentMenuList menuList;
		public string placeInput = "Fire1";
		public string cancelInput = "Cancel";
		public Material dragMaterial;

		PlacementMode placeState = PlacementMode.None;
		WorldEditTool lastTool;
		GameObject preview;
		GameObject agentPrefab;

		enum PlacementMode
		{
			None,
			DragAndDrop,
			ClickAndPlace
		}

		void Awake()
		{
			Debug.Assert(toolsList, $"{nameof(toolsList)} not defined in {name}.", this);
			Debug.Assert(menuList, $"{nameof(menuList)} not defined in {name}.", this);
		}

		void Update()
		{
			if (placeState == PlacementMode.None || !isMouseOverGame)
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

			if (world.TryRaycastBlocks(point, gameCamera.transform.forward, gameCamera.farClipPlane,
				out GridRaycastHit hit))
			{
				preview.transform.position =
					world.VoxelToWorld(hit.voxelIndex + hit.voxelNormal) - new Vector3(0, 0.5f, 0);
				preview.gameObject.SetActive(true);

				if (placeState == PlacementMode.ClickAndPlace && Input.GetButtonDown(placeInput))
				{
					EndPlacement();
					DragEndOrCancel();
				}
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
			if (placeState != PlacementMode.None || !menuList.currentAgent || !menuList.currentAgent.modelPrefab)
			{
				// Stop drag events
				eventData.pointerDrag = null;
				return;
			}

			lastTool = toolsList.currentTool;

			if (lastTool)
			{
				toolsList.DeselectToolWithoutUIUpdate();
			}

			placeState = PlacementMode.DragAndDrop;
			StartPlacement();
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			if (placeState != PlacementMode.DragAndDrop)
			{
				// Stop drag events
				eventData.pointerDrag = null;
			}
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			if (placeState != PlacementMode.DragAndDrop)
			{
				return;
			}

			// Release the item, successful end drag

			Vector3 point = gameCamera.ScreenToWorldPoint(Input.mousePosition);

			if (world.TryRaycastBlocks(point, gameCamera.transform.forward, gameCamera.farClipPlane,
				out GridRaycastHit hit))
			{
				preview.transform.position = world.VoxelToWorld(hit.voxelIndex + hit.voxelNormal) - new Vector3(0, 0.5f, 0);
				preview.gameObject.SetActive(true);

				EndPlacement();
			}

			DragEndOrCancel();
		}

		public override void OnToolSelectedChange(WorldEditTool last)
		{
			if (isSelected)
			{
				placeState = PlacementMode.ClickAndPlace;
				StartPlacement();
				lastTool = last;
			}
			else
			{
				DragEndOrCancel();
			}
		}

		public override void OnMouseOverChange()
		{
		}

		void DragEndOrCancel()
		{
			if (lastTool)
			{
				// Switch to last used tool
				toolsList.SelectTool(lastTool);
				lastTool = null;
			}
			else if (placeState == PlacementMode.ClickAndPlace)
			{
				// Deselect self
				toolsList.DeselectTool();
			}

			placeState = PlacementMode.None;

			if (preview)
			{
				Destroy(preview);
				preview = null;
			}
		}

		void StartPlacement()
		{
			preview = Instantiate(menuList.currentAgent.modelPrefab);
			agentPrefab = menuList.currentAgent.agentPrefab;

			if (dragMaterial)
			{
				foreach (Renderer child in preview.GetComponentsInChildren<Renderer>())
				{
					child.sharedMaterial = dragMaterial;
				}
			}
		}

		void EndPlacement()
		{
			Instantiate(agentPrefab, preview.transform.position, preview.transform.rotation);
		}
	}
}
