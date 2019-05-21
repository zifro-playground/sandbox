using PM;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox.UI.Config
{
	public sealed class ConfigAgentSettings : ConfigMenuItem,
		IPMAgentSelected
	{
		[FormerlySerializedAs("buttonText")]
		public Text buttonLabel;
		public string buttonTextFormat = "Inställningar för '{0}'";

		[Header("Settings input fields")]
		public InputField fieldName;

		Agent agent;

		new void Start()
		{
			base.Start();

			Debug.Assert(buttonLabel, $"{nameof(buttonLabel)} is not assigned for {name}.", this);
		}

		void OnEnable()
		{
			fieldName.onEndEdit.AddListener(AgentNameChanged);
		}

		void AgentNameChanged(string newName)
		{
			if (string.IsNullOrWhiteSpace(newName))
			{
				fieldName.text = agent.name;
				return;
			}

			agent.name = newName.Trim();
			fieldName.text = agent.name;
			UpdateButtonLabelFromAgent(agent);

			foreach (IPMAgentUpdated ev in UISingleton.FindInterfaces<IPMAgentUpdated>())
			{
				ev.OnPMAgentUpdated(agent);
			}
		}

		void IPMAgentSelected.OnPMAgentSelected(Agent selectedAgent)
		{
			UpdateButtonLabelFromAgent(selectedAgent);
			agent = selectedAgent;
		}

		void UpdateButtonLabelFromAgent(Agent newAgent)
		{
			buttonLabel.text = string.Format(buttonTextFormat, newAgent.name);
		}
	}
}
