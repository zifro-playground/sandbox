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
		public AgentMenuList agentMenu;
		public Text buttonLabel;
		public string buttonTextFormat = "Inställningar för '{0}'";

		[Multiline]
		public string deleteConfirmFormat = "Är du säker?\n\n" +
		                                    "'{0}' kommer att permanent tas bort. Detta går inte att ångra.";

		[Header("Settings input fields")]
		public InputField fieldName;
		public Dropdown fieldModel;
		public Button fieldDeleteButton;
		public ConfirmBox fieldDeleteConfirm;

		Agent agent;

		new void Start()
		{
			base.Start();

			Debug.Assert(agentMenu, $"{nameof(agentMenu)} is not assigned for '{name}'.", this);
			Debug.Assert(buttonLabel, $"{nameof(buttonLabel)} is not assigned for '{name}'.", this);
			Debug.Assert(fieldName, $"{nameof(fieldName)} is not assigned for '{name}'.", this);
			Debug.Assert(fieldModel, $"{nameof(fieldModel)} is not assigned for '{name}'.", this);
			Debug.Assert(fieldDeleteButton, $"{nameof(fieldDeleteButton)} is not assigned for '{name}'.", this);
			Debug.Assert(fieldDeleteConfirm, $"{nameof(fieldDeleteConfirm)} is not assigned for '{name}'.", this);

			fieldModel.options = ModelPreviewBank.main.modelPrefabs
				.Select(o => new Dropdown.OptionData(o.name))
				.ToList();
		}

		void OnEnable()
		{
			fieldName.onEndEdit.AddListener(OnAgentNameChanged);
			fieldModel.onValueChanged.AddListener(OnAgentModelChanged);
			fieldDeleteButton.onClick.AddListener(OnAgentDeletePressed);
		}

		void OnAgentDeletePressed()
		{
			string label = string.Format(deleteConfirmFormat, agent.name);
			fieldDeleteConfirm.ShowBox(label, OnAgentDeleteConfirmed, null);
		}

		void OnAgentDeleteConfirmed()
		{
			agentMenu.RemoveAgent(agentMenu.currentAgent);
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
