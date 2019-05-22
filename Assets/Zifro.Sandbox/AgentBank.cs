using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Mellis.Core.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;
using Zifro.Sandbox.ClrFunctions;
using Zifro.Sandbox.Entities;
using Zifro.Sandbox.UI;

namespace Zifro.Sandbox
{
	public sealed class AgentBank : MonoBehaviour
	{
		public static AgentBank main;

		public GameObject agentPrefab;

		public string defaultAgentName = "Unnamed";
		public GameObject defaultModelPrefab;

		[SerializeField]
		int _currentIndex = -1;

		[SerializeField]
		[FormerlySerializedAs("agents")]
		List<Agent> _agents = new List<Agent>();

		public Agent currentAgent
		{
			get => _currentIndex == -1 ? null : agents[_currentIndex];
			set => SelectAgent(value);
		}

		public IReadOnlyList<Agent> agents => _agents;

		public int currentIndex
		{
			get => _currentIndex;
			set => SelectAgent(value);
		}

		public delegate void AgentAllDeselectedDelegate(Agent deselectedAgent);
		public delegate void AgentDeselectedDelegate(Agent deselectedAgent);
		public delegate void AgentSelectedDelegate(Agent selectedAgent);
		public delegate void AgentUpdatedDelegate(Agent updatedAgent);
		public delegate void AgentRemovedDelegate(Agent removedAgent);

		/// <summary>
		/// Fires when an agent is deselected.
		/// When switching selected, fires just before the selection event <see cref="E:Zifro.Sandbox.AgentBank.AgentSelected" />.
		/// </summary>
		public event AgentDeselectedDelegate AgentDeselected;
		
		/// <summary>
		/// Fires when all agents are deselected in the menu.
		/// Means that when called, no agent is selected.
		/// Fires just after the individual deselection event <see cref="AgentDeselected"/>.
		/// </summary>
		public event AgentAllDeselectedDelegate AgentAllDeselected;
		
		/// <summary>
		/// Fires when an agent is selected in the menu.
		/// </summary>
		public event AgentSelectedDelegate AgentSelected;

		/// <summary>
		/// Fires when an agent is updated. This can be the name, model, etc.
		/// </summary>
		public event AgentUpdatedDelegate AgentUpdated;

		/// <summary>
		/// Fires when an agent is removed.
		/// </summary>
		public event AgentRemovedDelegate AgentRemoved;

		void OnEnable()
		{
			Debug.Assert(!main,
				$"There are multiple agent bank instances. '{(main ? main.name : string.Empty)}' and '{name}'.", this);
			main = this;
		}

		void Awake()
		{
			Debug.Assert(defaultModelPrefab, $"{nameof(defaultModelPrefab)} not defined in {name}.", this);
			Debug.Assert(agentPrefab, $"{nameof(agentPrefab)} not defined in {name}.", this);

			for (int i = agents.Count - 1; i >= 0; i--)
			{
				Agent agent = agents[i];

				if (agent == null)
				{
					Debug.LogWarning($"Agent at index {i} was null. Removing.", this);
					_agents.RemoveAt(i);
					continue;
				}

				Debug.Assert(agent.menuItem,
					$"{nameof(agent.menuItem)} is not assigned for agent '{agent.name}' (at index {i}).", this);
				SetAgentDefaults(agent);
			}
		}

		void Start()
		{
			if (_currentIndex != -1)
			{
				int tmpIndex = _currentIndex;
				_currentIndex = -1;
				SelectAgent(tmpIndex);
			}
		}

#if UNITY_EDITOR

		void OnValidate()
		{
			_currentIndex = Mathf.Clamp(_currentIndex, -1, _agents.Count);
		}

#endif

		public Agent GetAgent(AgentMenuItem menuItem)
		{
			return agents.FirstOrDefault(o => o.menuItem == menuItem);
		}

		public void SetAgentDefaults(Agent agent)
		{
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

		public void AddAgent(Agent newAgent)
		{
			Debug.Assert(!_agents.Contains(newAgent), "The agent is already added!", this);
			_agents.Add(newAgent);
		}

		public void RemoveAgent(Agent agent)
		{
			Debug.Assert(_agents.Contains(agent), "Agent does not exist in the bank.", this);

			if (currentAgent == agent)
			{
				DeselectAgent();
			}

			_agents.Remove(agent);
			OnAgentRemoved(agent);
		}

		public void SelectAgent(Agent agent)
		{
			Debug.Assert(agent != null, $"Cannot select null agent. Use {nameof(DeselectAgent)} instead.", this);
			int index = _agents.IndexOf(agent);
			Debug.Assert(index != -1, $"Agent does not exist in bank. Use {nameof(AddAgent)} first.", this);

			SelectAgent(index);
		}

		public void SelectAgent(int index)
		{
			Debug.Assert(_agents.Count > 0, $"There are no agents to select. Use {nameof(AddAgent)} first.", this);
			Debug.Assert(index >= 0 && index < _agents.Count, $"Agent selection index is out of range (0 - {_agents.Count-1}), got {index}.", this);

			if (index == _currentIndex)
			{
				return;
			}

			if (_currentIndex != -1)
			{
				OnAgentDeselected(currentAgent);
			}

			_currentIndex = index;

			OnAgentSelected(currentAgent);
		}

		public void DeselectAgent()
		{
			if (_currentIndex == -1)
			{
				return;
			}

			OnAgentDeselected(currentAgent);
			OnAgentAllDeselected(currentAgent);
			_currentIndex = -1;
		}

		public void UpdateAgent(Agent agent)
		{
			OnAgentUpdated(agent);
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

		void OnAgentDeselected(Agent deselectedAgent)
		{
			AgentDeselected?.Invoke(deselectedAgent);
		}

		void OnAgentAllDeselected(Agent deselectedAgent)
		{
			AgentAllDeselected?.Invoke(deselectedAgent);
		}

		void OnAgentSelected(Agent selectedAgent)
		{
			AgentSelected?.Invoke(selectedAgent);
		}

		void OnAgentUpdated(Agent updatedAgent)
		{
			AgentUpdated?.Invoke(updatedAgent);
		}

		void OnAgentRemoved(Agent removedAgent)
		{
			AgentRemoved?.Invoke(removedAgent);
		}
	}
}
