using System.Linq;
using UnityEngine;
using Zifro.Sandbox.UI.Config;

namespace Zifro.Sandbox.UI
{
	public sealed class ConfigMenuList : MenuList<ConfigMenuItem>
	{
		public AgentMenuList agentMenu;

#if UNITY_EDITOR

		new void OnValidate()
		{
			base.OnValidate();

			if (agentMenu)
			{
				SetMenuItemsActiveIfAgentIsSelected(agentMenu.currentAgent);
			}
		}

#endif

		void OnEnable()
		{
			Debug.Assert(agentMenu, $"{nameof(agentMenu)} is not assigned for {name}.", this);
			agentMenu.SelectedItem += AgentMenuOnSelectedItem;
		}

		public void SelectMenuItemAndSetActiveIfAgentIsSelected(bool isCurrentAgent)
		{
			if (currentItem)
			{
				bool state = isCurrentAgent == currentItem.isForAgents;
				if (!state)
				{
					DeselectTool();
				}
			}

			SetMenuItemsActiveIfAgentIsSelected(isCurrentAgent);

			if (!currentItem)
			{
				ConfigMenuItem selectNew = menuItems.FirstOrDefault(o => o.isForAgents == isCurrentAgent);
				Debug.Assert(selectNew, $"No selectable menu item found for agent state in '{name}'.", this);
				SelectItem(selectNew);
			}
		}

		private void SetMenuItemsActiveIfAgentIsSelected(bool isCurrentAgent)
		{
			foreach (ConfigMenuItem item in menuItems)
			{
				bool state = isCurrentAgent == item.isForAgents;
				if (state != item.gameObject.activeSelf)
				{
					item.gameObject.SetActive(state);
				}
			}
		}

		void AgentMenuOnSelectedItem(MenuItem item)
		{
			if (item is AgentMenuItem agentMenuItem)
			{
				SelectMenuItemAndSetActiveIfAgentIsSelected(true);

				foreach (ConfigAgentSettings agentSettings in menuItems.OfType<ConfigAgentSettings>())
				{
					agentSettings.OnAgentSelected(agentMenuItem.agent);
				}
			}
			else
			{
				SelectMenuItemAndSetActiveIfAgentIsSelected(false);
			}
		}
	}
}
