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

		void Start()
		{
			Debug.Assert(AgentBank.main, $"Missing main agents bank in {name}.", this);
			agent = AgentBank.main.GetAgent(this);
			Debug.Assert(agent != null, $"Unable to get agent in {name}.", this);
			Debug.Assert(label, $"{nameof(label)} is not assigned for {name}.", this);

			name = $"Agent '{agent?.name ?? "#unnamed"}'";
			label.text = agent.name;
		}

		public void OnMenuItemSelected()
		{
			foreach (IPMAgentSelected ev in UISingleton.FindInterfaces<IPMAgentSelected>())
			{
				ev.OnPMAgentSelected(agent);
			}
		}

		public void OnMenuItemDeselected()
		{
			foreach (IPMAgentDeselected ev in UISingleton.FindInterfaces<IPMAgentDeselected>())
			{
				ev.OnPMAgentDeselected(agent);
			}
		}
	}
}
