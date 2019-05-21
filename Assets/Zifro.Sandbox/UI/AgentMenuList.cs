using System;
using System.Collections.Generic;
using System.Linq;
using PM;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zifro.Sandbox.Entities;
using Zifro.Sandbox.UI.WorldEdit;
using Zifro.Sandbox.Utility;

namespace Zifro.Sandbox.UI
{
	public sealed class AgentMenuList : MenuList<MenuItem>,
		IPMPreCompilerStarted
	{
		public Button addButton;
		public InputField addInputField;
		public GameObject addInputPanel;
		public GameObject buttonPrefab;
		public ScrollRect scrollRect;
		public AgentDragAndDrop dragAndDropTool;

		public AgentMenuItem currentAgent => currentItem as AgentMenuItem;

		new void Start()
		{
			base.Start();
			Debug.Assert(addInputPanel, $"{nameof(addInputPanel)} is not assigned for {name}.", this);
			Debug.Assert(addButton, $"{nameof(addButton)} is not assigned for {name}.", this);
			Debug.Assert(addInputField, $"{nameof(addInputField)} is not assigned for {name}.", this);
			Debug.Assert(buttonPrefab, $"{nameof(buttonPrefab)} is not assigned for {name}.", this);
			Debug.Assert(scrollRect, $"{nameof(scrollRect)} is not assigned for {name}.", this);
			Debug.Assert(dragAndDropTool, $"{nameof(dragAndDropTool)} is not assigned for {name}.", this);

			addButton.onClick.AddListener(AddAgentViaUI);
			addInputField.AddTrigger(EventTriggerType.Submit, delegate { AddAgentViaUI(); });

			foreach (MenuItem item in menuItems)
			{
				item.button.onClick.AddListener(() => SelectItem(item));
			}

			foreach (AgentMenuItem agentMenuItem in menuItems.OfType<AgentMenuItem>())
			{
				agentMenuItem.SetTargetAgent(AgentBank.main.GetAgent(agentMenuItem));
			}

			if (!currentAgent)
			{
				foreach (IPMAgentAllDeselected ev in UISingleton.FindInterfaces<IPMAgentAllDeselected>())
				{
					ev.OnPMAgentAllDeselected(null);
				}
			}
		}

		protected override void OnSelectedMenuItem(MenuItem lastItem, MenuItem item)
		{
			if (lastItem is AgentMenuItem lastAgentMenuItem)
			{
				foreach (IPMAgentDeselected ev in UISingleton.FindInterfaces<IPMAgentDeselected>())
				{
					ev.OnPMAgentDeselected(lastAgentMenuItem.agent);
				}
			}

			if (item is AgentMenuItem agentMenuItem)
			{
				// Is agent
				agentMenuItem.SetTargetAgent(AgentBank.main.GetAgent(agentMenuItem));

				foreach (IPMAgentSelected ev in UISingleton.FindInterfaces<IPMAgentSelected>())
				{
					ev.OnPMAgentSelected(agentMenuItem.agent);
				}
			}
			else
			{
				// Is game settings
				if (lastItem is AgentMenuItem lastAgentMenuItem2)
				{
					foreach (IPMAgentAllDeselected ev in UISingleton.FindInterfaces<IPMAgentAllDeselected>())
					{
						ev.OnPMAgentAllDeselected(lastAgentMenuItem2.agent);
					}
				}
			}

			base.OnSelectedMenuItem(lastItem, item);
		}

		void AddAgentViaUI()
		{
			addInputPanel.SetActive(false);
			AddAgent(addInputField.text);
			addInputField.text = string.Empty;
			scrollRect.verticalNormalizedPosition = 0;
		}

		public void AddAgent(string agentName)
		{
			GameObject clone = Instantiate(buttonPrefab, transform);

			AgentMenuItem item = clone.GetComponent<AgentMenuItem>();
			item.button.onClick.AddListener(() => SelectItem(item));
			menuItems.Add(item);

			var agent = new Agent {
				menuItem = item,
				name = agentName
			};
			AgentBank.main.SetAgentDefaults(agent);
			AgentBank.main.agents.Add(agent);
			item.SetTargetAgent(agent);

			SelectItem(item);
		}

		void IPMPreCompilerStarted.OnPMPreCompilerStarted()
		{
			if (currentAgent)
			{
				currentAgent.agent.code = PMWrapper.mainCode;
			}
		}
	}
}
