using System;
using System.Collections.Generic;
using PM;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox.UI
{
	public class AgentMenuList : MonoBehaviour, IPMPreCompilerStarted
	{
		public Button addButton;
		public GameObject buttonPrefab;
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
			Debug.Assert(addButton, $"{nameof(addButton)} is not assigned for {name}.", this);
			Debug.Assert(buttonPrefab, $"{nameof(buttonPrefab)} is not assigned for {name}.", this);
		}

		void OnEnable()
		{
			bank = AgentBank.main;

			addButton.onClick.AddListener(AddAgent);
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
			}

			if (current is AgentMenuItem currentAgentMenu)
			{
				currentAgentMenu.OnMenuItemDeselected();
			}

			current = item;
			print($"selected a new agent yao, now {item.name} is my favorite");
			item.button.interactable = false;

			if (item is AgentMenuItem agentMenuItem)
			{
				agentMenuItem.OnMenuItemSelected();
			}
		}

		public void AddAgent()
		{
			print("I should add a new one yes.");
			GameObject clone = Instantiate(buttonPrefab, transform);

			AgentMenuItem item = clone.GetComponent<AgentMenuItem>();
			item.button.onClick.AddListener(() => SelectMenuItem(item));
			menuItems.Add(item);

			var agent = new Agent {
				menuItem = item,
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
