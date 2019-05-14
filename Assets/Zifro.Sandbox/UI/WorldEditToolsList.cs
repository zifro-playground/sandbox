using System;
using System.Collections.Generic;
using System.Linq;
using PM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zifro.Sandbox.UI.WorldEdit;
using Zifro.Sandbox.Utility;

namespace Zifro.Sandbox.UI
{
	public class WorldEditToolsList : MonoBehaviour,
		IPMCompilerStarted,
		IPMCompilerStopped
	{
		public VariableWindow variableWindow;
		public GridWorld gridWorld;

		public WorldEditTool currentTool;
		public List<WorldEditTool> tools;

		public EventTrigger gameWindowTrigger;

		void Awake()
		{
			Debug.Assert(tools.Count > 0, $"No tools found in {name}.", this);
			Debug.Assert(variableWindow, $"{nameof(variableWindow)} not defined in {name}.", this);
			Debug.Assert(gameWindowTrigger, $"{nameof(gameWindowTrigger)} not defined in {name}.", this);

			Debug.Assert(gridWorld, $"{nameof(gridWorld)} not assigned in {name}.", this);
		}

		void OnValidate()
		{
			if (currentTool && !tools.Contains(currentTool))
			{
				tools.Add(currentTool);
			}

			for (int i = tools.Count - 1; i >= 0; i--)
			{
				WorldEditTool tool = tools[i];
				if (!tool.button)
				{
					tool.button = tool.GetComponent<Button>();
					if (!tool.button)
					{
						Debug.LogAssertion($"Unable to find button for {tool.name}. Removing it from list.");
						tools.RemoveAt(i);
						continue;
					}
				}

				tool.button.interactable = tool != currentTool;
			}
		}

		void OnEnable()
		{
			Debug.Assert(Camera.main, "Missing main camera!.", this);

			foreach (WorldEditTool tool in tools)
			{
				// Initialize tool
				Debug.Assert(tool.button, $"Button for tool {tool.GetType().Name} is not assigned in \"{tool.name}\"", tool);

				tool.world = gridWorld;
				tool.gameCamera = Camera.main;

				// Register events
				tool.button.onClick.AddListener(() => SelectTool(tool));
			}

			SelectToolInternal(currentTool ? currentTool : tools.First(), true);

			// Register triggers
			gameWindowTrigger.AddTrigger(EventTriggerType.PointerEnter, delegate { SetItemsMouseOver(true); });
			gameWindowTrigger.AddTrigger(EventTriggerType.PointerExit, delegate { SetItemsMouseOver(false); });
		}

		void Update()
		{
			foreach (WorldEditTool tool in tools)
			{
				if (!tool.isSelected && Input.GetKeyDown(tool.hotKey))
				{
					SelectTool(tool);
					break;
				}
			}
		}

		public void SelectTool(WorldEditTool selectThis)
		{
			SelectToolInternal(selectThis, false);
		}

		void SelectToolInternal(WorldEditTool selectThis, bool force)
		{
			Debug.Assert(selectThis, "Tool cannot be null.");

			if (currentTool == selectThis && !force)
			{
				return;
			}

			foreach (WorldEditTool tool in tools)
			{
				if (tool == selectThis)
				{
					continue;
				}

				if (!tool.isSelected)
				{
					continue;
				}

				tool.button.interactable = true;
				tool.isSelected = false;
				tool.OnToolSelectedChange();
			}

			currentTool = selectThis;
			currentTool.button.interactable = false;
			currentTool.isSelected = true;
			currentTool.OnToolSelectedChange();

			variableWindow.gameObject.SetActive(false);
		}

		public void DeselectTool()
		{
			if (!currentTool)
			{
				return;
			}

			currentTool.button.interactable = true;
			currentTool.isSelected = false;
			currentTool.OnToolSelectedChange();
			currentTool = null;
			variableWindow.gameObject.SetActive(true);
		}

		void SetItemsMouseOver(bool state)
		{
			foreach (WorldEditTool tool in tools)
			{
				tool.isMouseOverGame = state;
				tool.OnMouseOverChange();
			}
		}

		void IPMCompilerStarted.OnPMCompilerStarted()
		{
			DeselectTool();
			foreach (WorldEditTool tool in tools)
			{
				tool.button.interactable = false;
			}

			enabled = false;
		}

		void IPMCompilerStopped.OnPMCompilerStopped(StopStatus status)
		{
			foreach (WorldEditTool tool in tools)
			{
				tool.button.interactable = true;
			}

			enabled = true;
		}
	}
}
