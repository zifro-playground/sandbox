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
	public class WorldEditToolsList : MenuList<WorldEditTool>
	{
		public EventTrigger gameWindowTrigger;

		new void Awake()
		{
			base.Awake();
			Debug.Assert(gameWindowTrigger, $"{nameof(gameWindowTrigger)} not defined in {name}.", this);
		}
		
		new void Start()
		{
			base.Start();

			Debug.Assert(Camera.main, $"Missing main camera in {name}.", this);
			Debug.Assert(GridWorld.main, $"Missing main grid world in {name}.", this);

			foreach (WorldEditTool tool in menuItems)
			{
				// Initialize tool
				Debug.Assert(tool.button, $"Button for tool {tool.GetType().Name} is not assigned in \"{tool.name}\"", tool);

				tool.world = GridWorld.main;
				tool.gameCamera = Camera.main;

				// Register events
				tool.button.onClick.AddListener(() => SelectItem(tool));
			}

			// Register triggers
			gameWindowTrigger.AddTrigger(EventTriggerType.PointerEnter, delegate { SetItemsMouseOver(true); });
			gameWindowTrigger.AddTrigger(EventTriggerType.PointerExit, delegate { SetItemsMouseOver(false); });
		}

		void Update()
		{
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			if (!currentSelectedGameObject || !currentSelectedGameObject.activeInHierarchy)
			{
				foreach (WorldEditTool tool in menuItems)
				{
					if (!tool.isSelected && Input.GetKeyDown(tool.hotKey))
					{
						SelectItem(tool);
						break;
					}
				}
			}
		}

		void SetItemsMouseOver(bool state)
		{
			foreach (WorldEditTool tool in menuItems)
			{
				tool.isMouseOverGame = state;
				tool.OnMouseOverChange();
			}
		}
	}
}
