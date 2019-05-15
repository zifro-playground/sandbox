using System;
using UnityEngine;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox.UI
{
	public class AgentMenuItem : MenuItem
	{
		public GameObject modelPrefab;
		public GameObject agentPrefab;

		Agent agent;

		new void Awake()
		{
			base.Awake();
			Debug.Assert(modelPrefab, $"{nameof(modelPrefab)} is not assigned for {name}.", this);
			Debug.Assert(agentPrefab, $"{nameof(agentPrefab)} is not assigned for {name}.", this);

			Debug.Assert(AgentBank.main, $"Missing main agents bank in {name}.", this);
			agent = AgentBank.main.GetAgent(this);
			Debug.Assert(agent != null, $"Unable to get agent in {name}.", this);

			name = $"Agent '{agent?.name ?? "#unnamed"}'";
		}
	}
}
