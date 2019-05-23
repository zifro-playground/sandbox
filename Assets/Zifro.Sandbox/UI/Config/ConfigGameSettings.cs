using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox.UI.Config
{
	public sealed class ConfigGameSettings : ConfigMenuItem
	{
		const string COLOR_FORMAT = "<color=#{0}>{1}</color>";
		
		public string optionNoInstancesToFollow = "<i>/inga instanser att följa/</i>";
		public string optionFollowNone = "<i>-följ ingen instans-</i>";
		public string optionDefaultFormat = "{0}: {1}";

		public Color optionColorDefault = new Color(.2f, .2f, .2f);
		public Color optionColorGrayedOut = new Color(.6f, .6f, .6f);

		[Header("Settings input fields")]
		public Dropdown fieldCameraFollow;

		[SerializeField, HideInInspector]
		List<AgentInstanceLookup> _optionLookups;

		void Awake()
		{
			AgentBank.main.AgentUpdated += OnAgentUpdated;
			AgentBank.main.AgentRemoved += OnAgentRemoved;
		}

		void OnDestroy()
		{
			AgentBank.main.AgentUpdated -= OnAgentUpdated;
			AgentBank.main.AgentRemoved -= OnAgentRemoved;
		}

		new void Start()
		{
			base.Start();

			Debug.Assert(fieldCameraFollow, $"{nameof(fieldCameraFollow)} not defined in '{name}'.", this);
		}

		void OnAgentUpdated(Agent updatedAgent)
		{
			UpdateCameraFollowDropDownFromAgents();
		}

		void OnAgentRemoved(Agent removedAgent)
		{
			UpdateCameraFollowDropDownFromAgents();
		}

		public override void OnMenuItemSelected(MenuItem lastItem)
		{
			base.OnMenuItemSelected(lastItem);

			UpdateCameraFollowDropDownFromAgents();
		}

		void UpdateCameraFollowDropDownFromAgents()
		{
			_optionLookups = new List<AgentInstanceLookup> {
				AgentInstanceLookup.empty
			};

			IReadOnlyList<Agent> allAgents = AgentBank.main.agents;
			for (int agentIndex = 0; agentIndex < allAgents.Count; agentIndex++)
			{
				if (allAgents[agentIndex].instances.Count > 0)
				{
					_optionLookups.AddRange(
						Enumerable.Range(0, allAgents[agentIndex].instances.Count)
							.Select(i => new AgentInstanceLookup(agentIndex, i)));
				}
			}

			if (_optionLookups.Count == 1)
			{
				fieldCameraFollow.interactable = false;
				fieldCameraFollow.options = new List<Dropdown.OptionData> {
					new Dropdown.OptionData(string.Format(COLOR_FORMAT,
						ColorUtility.ToHtmlStringRGB(optionColorGrayedOut),
						optionNoInstancesToFollow
					))
				};
			}
			else
			{
				fieldCameraFollow.interactable = true;
				fieldCameraFollow.options = _optionLookups
					.Skip(1).Select(o => 
						new Dropdown.OptionData(
							string.Format(COLOR_FORMAT,
								ColorUtility.ToHtmlStringRGB(optionColorDefault),
								string.Format(optionDefaultFormat, o.agent.name, o.instanceIndex + 1)
							)
						))
					.Prepend(new Dropdown.OptionData(string.Format(COLOR_FORMAT,
						ColorUtility.ToHtmlStringRGB(optionColorGrayedOut),
						optionFollowNone
					)))
					.ToList();
			}
		}

		[Serializable]
		struct AgentInstanceLookup
		{
			public int agentIndex;
			public int instanceIndex;

			public AgentInstanceLookup(int agentIndex, int instanceIndex)
			{
				this.agentIndex = agentIndex;
				this.instanceIndex = instanceIndex;
			}

			public Agent agent => agentIndex < 0 ? null : AgentBank.main.agents[agentIndex];
			public AgentInstance instance => isEmpty ? null : agent.instances[instanceIndex];

			public bool isEmpty => agentIndex < 0 || instanceIndex < 0;

			public static AgentInstanceLookup empty { get; } = new AgentInstanceLookup(-1, -1);

			public override string ToString()
			{
				return $"{agent.name}: {instanceIndex + 1}";
			}
		}
	}
}
