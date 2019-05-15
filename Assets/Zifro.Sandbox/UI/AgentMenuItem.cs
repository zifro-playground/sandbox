using System;
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
	}
}
