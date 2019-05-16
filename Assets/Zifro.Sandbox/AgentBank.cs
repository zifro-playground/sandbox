using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Mellis.Core.Interfaces;
using UnityEngine;
using Zifro.Sandbox.ClrFunctions;
using Zifro.Sandbox.Entities;
using Zifro.Sandbox.UI;

namespace Zifro.Sandbox
{
	public class AgentBank : MonoBehaviour
	{
		public static AgentBank main;

		public string defaultAgentName = "Unnamed";
		public GameObject defaultAgentPrefab;
		public GameObject defaultModelPrefab;

		public List<Agent> agents;

		void OnEnable()
		{
			Debug.Assert(!main,
				$"There are multiple agent bank instances. '{(main ? main.name : string.Empty)}' and '{name}'.", this);
			main = this;
		}

		void Awake()
		{
			Debug.Assert(defaultModelPrefab, $"{nameof(defaultModelPrefab)} not defined in {name}.", this);
			Debug.Assert(defaultAgentPrefab, $"{nameof(defaultAgentPrefab)} not defined in {name}.", this);

			for (int i = agents.Count - 1; i >= 0; i--)
			{
				Agent agent = agents[i];

				if (agent == null)
				{
					Debug.LogWarning($"Agent at index {i} was null. Removing.", this);
					agents.RemoveAt(i);
					continue;
				}

				Debug.Assert(agent.menuItem,
					$"{nameof(agent.menuItem)} is not assigned for agent '{agent.name}' (at index {i}).", this);
				SetAgentDefaults(agent);
			}
		}

		public Agent GetAgent(AgentMenuItem menuItem)
		{
			return agents.FirstOrDefault(o => o.menuItem == menuItem);
		}

		public void SetAgentDefaults(Agent agent)
		{
			if (!agent.agentPrefab)
			{
				agent.agentPrefab = defaultAgentPrefab;
			}

			if (!agent.modelPrefab)
			{
				agent.modelPrefab = defaultModelPrefab;
			}

			if (string.IsNullOrWhiteSpace(agent.name))
			{
				agent.name = defaultAgentName;
			}

			agent.name = agent.name.Trim();

			if (agent.instances == null)
			{
				agent.instances = new List<AgentInstance>();
			}
		}

		[Pure]
		public static IEmbeddedType[] GetAgentFunctions(AgentInstance instance)
		{
			return new IEmbeddedType[] {
				new IsBlockedFunction("blockerad_framåt", instance, Direction.Forward),
				new IsBlockedFunction("blockerad_bakåt", instance, Direction.Backward),
				new IsBlockedFunction("blockerad_höger", instance, Direction.Right),
				new IsBlockedFunction("blockerad_vänster", instance, Direction.Left),
				new IsBlockedFunction("blockerad_upp", instance, Direction.Up),
				new IsBlockedFunction("blockerad_ner", instance, Direction.Down),

				new GetKeyDownFunction("knapp_höger", KeyCode.RightArrow),
				new GetKeyDownFunction("knapp_vänster", KeyCode.LeftArrow),
				new GetKeyDownFunction("knapp_upp", KeyCode.UpArrow),
				new GetKeyDownFunction("knapp_ner", KeyCode.DownArrow),
				new GetKeyDownFunction("knapp_mellanslag", KeyCode.Space),

				new RotateFunction("sväng_höger", instance, Rotation.Right),
				new RotateFunction("sväng_vänster", instance, Rotation.Left),

				new MoveFunction("gå_framåt", instance, Direction.Forward),
				new MoveFunction("gå_bakåt", instance, Direction.Backward),
				new MoveFunction("gå_höger", instance, Direction.Right),
				new MoveFunction("gå_vänster", instance, Direction.Left),
				new MoveFunction("gå_upp", instance, Direction.Up),
				new MoveFunction("gå_ner", instance, Direction.Down),

				new MoveUntilBlockedFunction("gå_framåt_blockerad", instance, Direction.Forward),
				new MoveUntilBlockedFunction("gå_bakåt_blockerad", instance, Direction.Backward),
				new MoveUntilBlockedFunction("gå_höger_blockerad", instance, Direction.Right),
				new MoveUntilBlockedFunction("gå_vänster_blockerad", instance, Direction.Left),
				new MoveUntilBlockedFunction("gå_upp_blockerad", instance, Direction.Up),
				new MoveUntilBlockedFunction("gå_ner_blockerad", instance, Direction.Down)

				//new MoveFunction("gå_norr", instance, Direction.North),
				//new MoveFunction("gå_väst", instance, Direction.West),
				//new MoveFunction("gå_syd", instance, Direction.South),
				//new MoveFunction("gå_öst", instance, Direction.East)
			};
		}
	}
}
