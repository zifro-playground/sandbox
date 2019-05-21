using UnityEngine;

namespace Zifro.Sandbox.UI.Config
{
	public class ConfigMenuItem : MenuItem
	{
		public bool isForAgents;
		public GameObject activeWhenSelected;

#if UNITY_EDITOR
		
		AgentMenuList _agentMenu;
		void OnValidate()
		{
			if (!_agentMenu)
			{
				_agentMenu = FindObjectOfType<AgentMenuList>();
				if (!_agentMenu)
				{
					return;
				}
			}

			bool state = _agentMenu.currentAgent == isForAgents;
			if (state != gameObject.activeSelf)
			{
				gameObject.SetActive(state);
			}
		}

#endif
		
		public override void OnMenuItemSelected(MenuItem lastItem)
		{
			if (activeWhenSelected)
			{
				activeWhenSelected.SetActive(true);
			}
		}

		public override void OnMenuItemDeselected()
		{
			if (activeWhenSelected)
			{
				activeWhenSelected.SetActive(false);
			}
		}

	}
}
