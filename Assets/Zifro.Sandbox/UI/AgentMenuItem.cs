using System;
using PM;
using UnityEngine;
using UnityEngine.UI;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox.UI
{
	public sealed class AgentMenuItem : MenuItem,
		IPMAgentUpdated
	{
		[NonSerialized]
		public Agent agent;

		public Text label;
		public RawImage preview;

		new void Start()
		{
			base.Start();
			Debug.Assert(agent != null, $"{nameof(agent)} is not assigned for {name} (Should have been assigned with {nameof(SetTargetAgent)}).", this);
			Debug.Assert(label, $"{nameof(label)} is not assigned for {name}.", this);
			Debug.Assert(preview, $"{nameof(preview)} is not assigned for {name}.", this);
		}

		public void SetTargetAgent(Agent newAgent)
		{
			agent = newAgent;
			name = $"Agent '{agent.name}'";
			label.text = agent.name;
			preview.texture = ModelPreviewBank.main.GetOrCreateTexture(agent.modelPrefab);
		}

		public override void OnMenuItemSelected(MenuItem lastItem)
		{
			PMWrapper.mainCode = agent.code;
			PMWrapper.preCode = $"# Kod för \"{agent.name}\"";
		}

		public override void OnMenuItemDeselected()
		{
			agent.code = PMWrapper.mainCode;
		}

		void IPMAgentUpdated.OnPMAgentUpdated(Agent updatedAgent)
		{
			if (updatedAgent != agent)
			{
				return;
			}

			name = $"Agent '{updatedAgent.name}'";
			label.text = updatedAgent.name;
			preview.texture = ModelPreviewBank.main.GetOrCreateTexture(updatedAgent.modelPrefab);

			foreach (AgentInstance instance in agent.instances)
			{
				// Assuming the model is it's first child
				Transform parent = instance.transform;
				Destroy(parent.GetChild(0).gameObject);
				Instantiate(agent.modelPrefab, parent.position, parent.rotation, parent);
			}
		}
	}
}
