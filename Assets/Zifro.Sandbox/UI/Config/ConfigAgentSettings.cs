using System.Collections.Generic;
using System.Linq;
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
		public Dropdown fieldModel;

		Agent agent;

		new void Start()
		{
			base.Start();

			Debug.Assert(buttonLabel, $"{nameof(buttonLabel)} is not assigned for '{name}'.", this);
			Debug.Assert(fieldName, $"{nameof(fieldName)} is not assigned for '{name}'.", this);
			Debug.Assert(fieldModel, $"{nameof(fieldModel)} is not assigned for '{name}'.", this);

			fieldModel.options = ModelPreviewBank.main.modelPrefabs
				.Select(o => new Dropdown.OptionData(o.name))
				.ToList();
		}

		void OnEnable()
		{
			fieldName.onEndEdit.AddListener(OnAgentNameChanged);
			fieldModel.onValueChanged.AddListener(OnAgentModelChanged);
		}

		void OnAgentModelChanged(int index)
		{
			agent.modelPrefab = ModelPreviewBank.main.modelPrefabs[index];

			SendAgentUpdatedMessage();
		}

		void OnAgentNameChanged(string newName)
		{
			if (string.IsNullOrWhiteSpace(newName))
			{
				fieldName.text = agent.name;
				return;
			}

			agent.name = newName.Trim();
			fieldName.text = agent.name;
			UpdateButtonLabelFromAgent(agent);

			SendAgentUpdatedMessage();
		}

		void IPMAgentSelected.OnPMAgentSelected(Agent selectedAgent)
		{
			agent = selectedAgent;

			UpdateButtonLabelFromAgent(agent);
			fieldName.text = agent.name;
			fieldModel.value = ModelPreviewBank.main.modelPrefabs.IndexOf(agent.modelPrefab);
		}

		void UpdateButtonLabelFromAgent(Agent newAgent)
		{
			buttonLabel.text = string.Format(buttonTextFormat, newAgent.name);
		}

		void SendAgentUpdatedMessage()
		{
			foreach (IPMAgentUpdated ev in UISingleton.FindInterfaces<IPMAgentUpdated>())
			{
				ev.OnPMAgentUpdated(agent);
			}
		}
	}
}
