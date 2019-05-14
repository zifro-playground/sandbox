using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zifro.Sandbox.Entities;
using Zifro.Sandbox.UI;

namespace Zifro.Sandbox
{
	public class AgentBank : MonoBehaviour
	{
		public static AgentBank main;

		public List<Agent> agents;

		void Awake()
		{
			main = this;

			for (int i = agents.Count - 1; i >= 0; i--)
			{
				Agent agent = agents[i];

				if (agent == null)
				{
					Debug.LogWarning($"Agent at index {i} was null. Removing.", this);
					agents.RemoveAt(i);
					continue;
				}

				Debug.Assert(!string.IsNullOrWhiteSpace(agent.name), $"Agent at index {i} is missing name.", this);
				Debug.Assert(agent.menuItem, $"Agent {agent.name} at index {i} is missing menu link.", this);

				agent.name = agent.name.Trim();

				if (agent.instances == null)
				{
					agent.instances = new List<AgentInstance>();
				}
			}
		}

		public Agent GetAgent(AgentMenuItem menuItem)
		{
			return agents.FirstOrDefault(o => o.menuItem == menuItem);
		}
	}
}
