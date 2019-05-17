using System;
using PM;
using UnityEngine;
using UnityEngine.UI;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox.UI
{
	public class AgentMenuItem : MenuItem
	{
		[NonSerialized]
		public Agent agent;

		public Text label;
		public RawImage preview;

		void Start()
		{
			Debug.Assert(agent != null, $"{nameof(agent)} is not assigned for {name} (Should have been assigned by {nameof(AgentMenuList)}.{nameof(AgentMenuList.SelectMenuItem)}).", this);
			Debug.Assert(label, $"{nameof(label)} is not assigned for {name}.", this);
			Debug.Assert(preview, $"{nameof(preview)} is not assigned for {name}.", this);

			name = $"Agent '{agent?.name ?? "#unnamed"}'";
			label.text = agent.name;
		}

		public void OnMenuItemSelected()
		{
			foreach (IPMAgentSelected ev in UISingleton.FindInterfaces<IPMAgentSelected>())
			{
				ev.OnPMAgentSelected(agent);
			}

			PMWrapper.mainCode = agent.code;
			PMWrapper.preCode = $"# Kod för \"{agent.name}\"";

			preview.texture = ModelPreviewBank.main.GetOrCreateTexture(agent.modelPrefab);
		}

		public void OnMenuItemDeselected()
		{
			foreach (IPMAgentDeselected ev in UISingleton.FindInterfaces<IPMAgentDeselected>())
			{
				ev.OnPMAgentDeselected(agent);
			}

			agent.code = PMWrapper.mainCode;
		}
	}
}
