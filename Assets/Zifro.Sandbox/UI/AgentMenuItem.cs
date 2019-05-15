using System;
using UnityEngine;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox.UI
{
	public class AgentMenuItem : MenuItem
	{
		[NonSerialized]
		public Agent agent;

		new void Awake()
		{
			base.Awake();

			Debug.Assert(AgentBank.main, $"Missing main agents bank in {name}.", this);
			agent = AgentBank.main.GetAgent(this);
			Debug.Assert(agent != null, $"Unable to get agent in {name}.", this);

			name = $"Agent '{agent?.name ?? "#unnamed"}'";
		}
	}
}
