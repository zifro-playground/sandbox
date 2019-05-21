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
		AgentBank bank;

		new void Start()
		{
			base.Start();
			Debug.Assert(addInputPanel, $"{nameof(addInputPanel)} is not assigned for {name}.", this);
			Debug.Assert(addButton, $"{nameof(addButton)} is not assigned for {name}.", this);
			Debug.Assert(addInputField, $"{nameof(addInputField)} is not assigned for {name}.", this);
			Debug.Assert(buttonPrefab, $"{nameof(buttonPrefab)} is not assigned for {name}.", this);
			Debug.Assert(scrollRect, $"{nameof(scrollRect)} is not assigned for {name}.", this);
			Debug.Assert(dragAndDropTool, $"{nameof(dragAndDropTool)} is not assigned for {name}.", this);

			bank = AgentBank.main;

			addButton.onClick.AddListener(AddAgentViaUI);
			addInputField.AddTrigger(EventTriggerType.Submit, delegate { AddAgentViaUI(); });

			foreach (MenuItem item in menuItems)
			{
				item.button.onClick.AddListener(() => SelectItem(item));
			}

			foreach (AgentMenuItem agentMenuItem in menuItems.OfType<AgentMenuItem>())
			{
				agentMenuItem.SetTargetAgent(bank.GetAgent(agentMenuItem));
			}
		}

		protected override void OnSelectedMenuItem(MenuItem item)
		{
			base.OnSelectedMenuItem(item);

			if (item is AgentMenuItem agentMenuItem)
			{
				// Is agent
				agentMenuItem.SetTargetAgent(bank.GetAgent(agentMenuItem));
				dragAndDropTool.ShowTool(agentMenuItem.agent);
			}
			else
			{
				// Is game settings
				dragAndDropTool.HideTool();
			}
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

			bank.SetAgentDefaults(agent);
			bank.agents.Add(agent);
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
