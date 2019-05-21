using System;
using PM;
using UnityEngine;
using UnityEngine.UI;
using Zifro.Sandbox.Entities;

namespace Zifro.Sandbox.UI
{
	public sealed class AgentMenuItem : MenuItem
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
			foreach (IPMAgentSelected ev in UISingleton.FindInterfaces<IPMAgentSelected>())
			{
				ev.OnPMAgentSelected(agent);
			}

			PMWrapper.mainCode = agent.code;
			PMWrapper.preCode = $"# Kod för \"{agent.name}\"";
		}

		public override void OnMenuItemDeselected()
		{
			foreach (IPMAgentDeselected ev in UISingleton.FindInterfaces<IPMAgentDeselected>())
			{
				ev.OnPMAgentDeselected(agent);
			}

			agent.code = PMWrapper.mainCode;
		}
	}
}
