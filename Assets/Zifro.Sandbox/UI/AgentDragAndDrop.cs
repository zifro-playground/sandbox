using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Zifro.Sandbox.Entities;
using Zifro.Sandbox.UI.WorldEdit;

namespace Zifro.Sandbox.UI
{
	public class AgentDragAndDrop : WorldEditTool,
		IBeginDragHandler,
		IDragHandler,
		IEndDragHandler,
		IPointerClickHandler
	{
		public WorldEditToolsList toolsList;
		public AgentMenuList menuList;
		public string placeInput = "Fire1";
		public string cancelInput = "Cancel";
		public Material dragMaterial;

		PlacementMode placeState = PlacementMode.None;
		WorldEditTool lastTool;
		GameObject preview;
		Agent draggedAgent;
		bool isActivatingClickAndDragThisFrame;

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
				preview.transform.position = world.VoxelToWorld(hit.voxelIndex + hit.voxelNormal);
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
			if (placeState != PlacementMode.None || !menuList.currentAgent)
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
				preview.transform.position = world.VoxelToWorld(hit.voxelIndex + hit.voxelNormal);

				EndPlacement();
			}

			DragEndOrCancel();
		}

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			if (isSelected && 
			    placeState == PlacementMode.ClickAndPlace &&
			    !isActivatingClickAndDragThisFrame)
			{
				DragEndOrCancel();
			}
		}

		IEnumerator DisableClickAndDragBoolNextFrame()
		{
			yield return null;
			isActivatingClickAndDragThisFrame = false;
		}

		public override void OnToolSelectedChange(WorldEditTool last)
		{
			if (isSelected && placeState == PlacementMode.None)
			{
				placeState = PlacementMode.ClickAndPlace;
				StartPlacement();
				lastTool = last;
				button.interactable = true;
				isActivatingClickAndDragThisFrame = true;
				StartCoroutine(DisableClickAndDragBoolNextFrame());
				EventSystem.current.SetSelectedGameObject(gameObject);
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
			if (!toolsList.isSelecting)
			{
				if (lastTool)
				{
					// Switch to last used tool
					toolsList.SelectTool(lastTool);
				}
				else if (placeState == PlacementMode.ClickAndPlace)
				{
					// Deselect self
					toolsList.DeselectTool();
				}
			}

			lastTool = null;
			placeState = PlacementMode.None;

			if (preview)
			{
				Destroy(preview);
				preview = null;
			}
		}

		void StartPlacement()
		{
			draggedAgent = menuList.currentAgent.agent;
			preview = Instantiate(draggedAgent.modelPrefab);

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
			Vector3 position = preview.transform.position;
			GameObject clone = Instantiate(AgentBank.main.agentPrefab, position, preview.transform.rotation, AgentBank.main.transform);
			Instantiate(draggedAgent.modelPrefab, clone.transform.position, clone.transform.rotation, clone.transform);

			AgentInstance agentInstance = clone.GetComponent<AgentInstance>();
			agentInstance.fractionPosition = (FractionVector3)position;

			draggedAgent.instances.Add(agentInstance);
			draggedAgent = null;
		}
	}
}
