using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PM;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox.UI.Config
{
	public sealed class ConfigAgentSettings : ConfigMenuItem
	{
		public AgentMenuList agentMenu;
		public Text buttonLabel;
		public string buttonTextFormat = "Inställningar för '{0}'";

		public ConfirmBox confirmBoxDanger;

		[Multiline]
		public string deleteConfirmFormat =
			"Är du säker?\n\n" +
			"'{0}' kommer att permanent tas bort. Detta går inte att ångra.";

		[Multiline]
		public string shrinkMaxCountFormat =
			"Du sänker högsta antal instanser till {0} st, men det finns {1} st utplacerade redan.\n\n" +
			"Vill du ta bort dem överflödiga? Detta går inte att ångra.";

		[Header("Settings input fields")]
		public InputField fieldName;

		public Dropdown fieldModel;
		public InputField fieldMaxCount;
		public Button fieldDeleteButton;

		Agent agent;

		new void Start()
		{
			base.Start();

			Debug.Assert(agentMenu, $"{nameof(agentMenu)} is not assigned for '{name}'.", this);
			Debug.Assert(buttonLabel, $"{nameof(buttonLabel)} is not assigned for '{name}'.", this);
			Debug.Assert(fieldName, $"{nameof(fieldName)} is not assigned for '{name}'.", this);
			Debug.Assert(fieldModel, $"{nameof(fieldModel)} is not assigned for '{name}'.", this);
			Debug.Assert(fieldMaxCount, $"{nameof(fieldMaxCount)} is not assigned for '{name}'.", this);
			Debug.Assert(fieldDeleteButton, $"{nameof(fieldDeleteButton)} is not assigned for '{name}'.", this);
			Debug.Assert(confirmBoxDanger, $"{nameof(confirmBoxDanger)} is not assigned for '{name}'.", this);

			fieldModel.options = ModelPreviewBank.main.modelPrefabs
				.Select(o => new Dropdown.OptionData(o.name))
				.ToList();
		}

		void Awake()
		{
			AgentBank.main.AgentSelected += OnAgentSelected;
		}

		void OnDestroy()
		{
			AgentBank.main.AgentSelected -= OnAgentSelected;
		}

		void OnEnable()
		{
			fieldName.onEndEdit.AddListener(OnAgentNameChanged);
			fieldModel.onValueChanged.AddListener(OnAgentModelChanged);
			fieldDeleteButton.onClick.AddListener(OnAgentDeletePressed);
			fieldMaxCount.onEndEdit.AddListener(OnAgentMaxInstanceCountChanged);
		}

		void OnAgentMaxInstanceCountChanged(string arg0)
		{
			if (string.IsNullOrWhiteSpace(arg0))
			{
				agent.maxInstanceCount = -1;
				UpdateFieldMaxInstanceCountFromAgent();
				return;
			}

			if (!int.TryParse(arg0, NumberStyles.Any, CultureInfo.CurrentUICulture, out int max))
			{
				UpdateFieldMaxInstanceCountFromAgent();
				return;
			}

			if (max <= 0)
			{
				max = -1;
			}

			if (max >= agent.instances.Count || max == -1)
			{
				agent.maxInstanceCount = max;
				UpdateFieldMaxInstanceCountFromAgent();
				return;
			}

			// Too many instances in the world. We have to purge
			string label = string.Format(
				CultureInfo.CurrentUICulture,
				shrinkMaxCountFormat,
				arg0: max,
				arg1: agent.instances.Count);

			confirmBoxDanger.ShowBox(label, () => OnAgentMaxInstanceCountPurge(max), null);
		}

		void OnAgentMaxInstanceCountPurge(int max)
		{
			for (int i = agent.instances.Count - 1; i >= max; i--)
			{
				Destroy(agent.instances[i].gameObject);
				agent.instances.RemoveAt(i);
			}

			agent.maxInstanceCount = max;
			UpdateFieldMaxInstanceCountFromAgent();
		}

		void OnAgentDeletePressed()
		{
			string label = string.Format(deleteConfirmFormat, agent.name);
			confirmBoxDanger.ShowBox(label, OnAgentDeleteConfirmed, null);
		}

		void OnAgentDeleteConfirmed()
		{
			agentMenu.RemoveAgent(agentMenu.currentAgent);
		}

		void OnAgentModelChanged(int index)
		{
			agent.modelPrefab = ModelPreviewBank.main.modelPrefabs[index];

			AgentBank.main.UpdateAgent(agent);
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

			AgentBank.main.UpdateAgent(agent);
		}

		void OnAgentSelected(Agent selectedAgent)
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

		void UpdateFieldMaxInstanceCountFromAgent()
		{
			fieldMaxCount.text = agent.maxInstanceCount > 0
				? agent.maxInstanceCount.ToString(CultureInfo.CurrentUICulture)
				: string.Empty;
		}

		public override void OnMenuItemDeselected()
		{
			base.OnMenuItemDeselected();
			confirmBoxDanger.Cancel();
		}
	}
}
