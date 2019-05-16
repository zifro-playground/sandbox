using System;
using System.Collections.Generic;
using PM;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zifro.Sandbox.Entities;
using Zifro.Sandbox.Utility;

namespace Zifro.Sandbox.UI
{
	public class AgentMenuList : MonoBehaviour, IPMPreCompilerStarted
	{
		public Button addButton;
		public InputField addInputField;
		public GameObject addInputPanel;
		public GameObject buttonPrefab;
		public ScrollRect scrollRect;
		public GameObject dragAndDropTool;
		public Text dragAndDropLabel;
		[Space]
		public MenuItem current;
		public List<MenuItem> menuItems;

		public AgentMenuItem currentAgent => current as AgentMenuItem;
		AgentBank bank;

		void OnValidate()
		{
			if (current && !menuItems.Contains(current))
			{
				menuItems.Add(current);
			}

			for (int i = menuItems.Count - 1; i >= 0; i--)
			{
				if (!menuItems[i])
				{
					Debug.LogAssertion($"Menu item at index {i} was null. Removing it from list.");
					menuItems.RemoveAt(i);
					continue;
				}

				MenuItem menuItem = menuItems[i];
				if (!menuItem.button)
				{
					menuItem.button = menuItem.GetComponent<Button>();
					if (!menuItem.button)
					{
						Debug.LogAssertion($"Unable to find button for {menuItem.name}. Removing it from list.");
						menuItems.RemoveAt(i);
						continue;
					}
				}

				menuItem.button.interactable = menuItem != current;
			}
		}

		void Awake()
		{
			Debug.Assert(addInputPanel, $"{nameof(addInputPanel)} is not assigned for {name}.", this);
			Debug.Assert(addButton, $"{nameof(addButton)} is not assigned for {name}.", this);
			Debug.Assert(addInputField, $"{nameof(addInputField)} is not assigned for {name}.", this);
			Debug.Assert(buttonPrefab, $"{nameof(buttonPrefab)} is not assigned for {name}.", this);
			Debug.Assert(scrollRect, $"{nameof(scrollRect)} is not assigned for {name}.", this);
			Debug.Assert(dragAndDropTool, $"{nameof(dragAndDropTool)} is not assigned for {name}.", this);
			Debug.Assert(dragAndDropLabel, $"{nameof(dragAndDropLabel)} is not assigned for {name}.", this);
		}

		void OnEnable()
		{
			bank = AgentBank.main;

			addButton.onClick.AddListener(AddAgentViaUI);
			addInputField.AddTrigger(EventTriggerType.Submit, delegate { AddAgentViaUI(); });

			foreach (MenuItem item in menuItems)
			{
				item.button.onClick.AddListener(() => SelectMenuItem(item));
			}
		}

		void Start()
		{
			SelectMenuItemInternal(current, true);
		}

		public void SelectMenuItem(MenuItem agent)
		{
			SelectMenuItemInternal(agent, false);
		}

		void SelectMenuItemInternal(MenuItem item, bool force)
		{
			Debug.Assert(item, "Agent cannot be null.");
			if (current == item && !force)
			{
				return;
			}

			if (current)
			{
				current.button.interactable = true;
				if (!force && current is AgentMenuItem currentAgentMenu)
				{
					currentAgentMenu.OnMenuItemDeselected();
				}
			}

			current = item;
			item.button.interactable = false;

			if (item is AgentMenuItem agentMenuItem)
			{
				// Is agent
				if (agentMenuItem.agent == null)
				{
					agentMenuItem.agent = bank.GetAgent(agentMenuItem);
				}

				dragAndDropLabel.text = agentMenuItem.agent.name;
				dragAndDropTool.SetActive(true);
				agentMenuItem.OnMenuItemSelected();
			}
			else
			{
				// Is game settings
				dragAndDropTool.SetActive(false);
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
			item.button.onClick.AddListener(() => SelectMenuItem(item));
			menuItems.Add(item);

			var agent = new Agent {
				menuItem = item,
				name = agentName
			};

			bank.SetAgentDefaults(agent);
			bank.agents.Add(agent);

			SelectMenuItem(item);
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
