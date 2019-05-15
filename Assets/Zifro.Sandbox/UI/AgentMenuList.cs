using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Zifro.Sandbox.UI
{
	public class AgentMenuList : MonoBehaviour, IPMPreCompilerStarted
	{
		public Button addButton;
		public MenuItem current;
		public AgentMenuItem currentAgent => current as AgentMenuItem;
		public List<MenuItem> menuItems;

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
		}

		void OnEnable()
		{
			addButton.onClick.AddListener(AddAgent);
			foreach (MenuItem item in menuItems)
			{
				item.button.onClick.AddListener(() => SelectMenuItem(item));
			}
		}

		public void SelectMenuItem(MenuItem agent)
		{
			Debug.Assert(agent, "Agent cannot be null.");
			if (current == agent)
			{
				return;
			}

			if (current)
			{
				current.button.interactable = true;
			}

			current = agent;
			print($"selected a new agent yao, now {agent.name} is my favorite");
			agent.button.interactable = false;
		}

		public void AddAgent()
		{
			print("I should add a new one yes.");
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
